using Microsoft.Extensions.Options;
using MoveIT.Transfer.Task.Application.Domain.Exceptions;
using MoveIT.Transfer.Task.Application.Domain.Helpers;
using MoveIT.Transfer.Task.Application.Domain.Helpers.TokenManager;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace MoveIT.Transfer.Task.Application.Domain.Services.HttpClientService
{
	public class HttpClientService : IHttpClientService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ITokenManager _tokenManager;
		private readonly HttpClient _httpClient;
		private readonly string _moveItApiUrl;

		public HttpClientService(
			IHttpClientFactory httpClientFactory,
			IOptions<AppSettings> appSettings,
			ITokenManager tokenManager)
		{
			_httpClientFactory = httpClientFactory;
			_tokenManager = tokenManager;
			_httpClient = _httpClientFactory.CreateClient();
			_moveItApiUrl = appSettings.Value.MOVE_IT_API_URL;
		}

		/// <summary>
		/// Abstract HTTP Get method.
		/// </summary>
		/// <typeparam name="T">Type of expected response.</typeparam>
		/// <param name="url">Partial endpoint url. Example : folders/folderName/</param>
		/// <param name="headers">Additional headers.</param>
		/// <returns><b><typeparamref name="T"/></b></returns>
		/// <exception cref="HttpClientServiceException"></exception>
		public async Task<T> GetAsync<T>(string url, Dictionary<string, string>? headers = null)
		{
			var fullUrl = _moveItApiUrl + url;

			var request = CreateRequest(HttpMethod.Get, fullUrl, headers);

			return await SendRequestAsync<T>(request);
		}

		/// <summary>
		/// Abstract HTTP Post method.
		/// </summary>
		/// <typeparam name="T">Type of expected response.</typeparam>
		/// <param name="url">Partial endpoint url. Example : folders/folderName/</param>
		/// <param name="payload">Body of the request.</param>
		/// <param name="headers">Additional headers.</param>
		/// <returns><b><typeparamref name="T"/></b></returns>
		/// <exception cref="HttpClientServiceException"></exception>
		public async Task<T> PostAsync<T>(
			string url,
			IEnumerable<KeyValuePair<string, string>> payload,
			Dictionary<string, string>? headers = null)
		{
			var fullUrl = _moveItApiUrl + url;

			var content = new FormUrlEncodedContent(payload);
			var request = CreateRequest(HttpMethod.Post, fullUrl, headers, content);

			return await SendRequestAsync<T>(request);
		}

		/// <summary>
		/// Abstract HTTP Patch method.
		/// </summary>
		/// <typeparam name="T">Type of expected response.</typeparam>
		/// <param name="url">Partial endpoint url. Example : folders/folderName/</param>
		/// <param name="payload">Body of the request.</param>
		/// <param name="headers">Additional headers.</param>
		/// <returns><b><typeparamref name="T"/></b></returns>
		/// <exception cref="HttpClientServiceException"></exception>
		public async Task<T> PatchAsync<T>(
			string url,
			IEnumerable<KeyValuePair<string, string>> payload,
			Dictionary<string, string>? headers = null)
		{
			var fullUrl = _moveItApiUrl + url;

			var content = new StringContent(JsonConvert.SerializeObject(payload));
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var request = CreateRequest(HttpMethod.Patch, fullUrl, headers, content);

			return await SendRequestAsync<T>(request);
		}

		/// <summary>
		/// Abstract HTTP Delete method.
		/// </summary>
		/// <param name="url">Partial endpoint url. Example : folders/folderName/</param>
		/// <param name="headers">Additional headers.</param>
		/// <returns>Nothing.</returns>
		/// <exception cref="HttpClientServiceException"></exception>
		public async System.Threading.Tasks.Task DeleteAsync(string url, Dictionary<string, string>? headers = null)
		{
			var fullUrl = _moveItApiUrl + url;

			var request = CreateRequest(HttpMethod.Delete, fullUrl, headers);

			await SendRequestAsync(request);
		}

		public async System.Threading.Tasks.Task PostFileAsync(
			string url,
			string filePath,
			string fileName,
			string parameterName = "file")
		{
			var fullUrl = _moveItApiUrl + url;
			await PostFileInSingleRequestAsync(fullUrl, filePath, fileName, parameterName);
		}

		private async System.Threading.Tasks.Task PostFileInSingleRequestAsync(
			string fullUrl,
			string filePath,
			string fileName,
			string parameterName)
		{
			await using var fileStream = File.OpenRead(filePath);

			var content = new MultipartFormDataContent();
			var fileContent = new StreamContent(fileStream);

			fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
			{
				Name = parameterName,
				FileName = fileName
			};

			content.Headers.Add("hash", await GenerateDigest(filePath));
			content.Headers.Add("hashtype", "sha-256");
			content.Add(fileContent);

			var requestMessage = CreateRequest(HttpMethod.Post, fullUrl, content: content);

			await SendRequestAsync(requestMessage);
		}

		private async System.Threading.Tasks.Task PostFileInChunksAsync(
			string fullUrl,
			string filePath,
			string fileName,
			string parameterName)
		{
			throw new NotImplementedException();
			const int chunkSize = 1024 * 1024;

			await using var fileStream = File.OpenRead(filePath);

			var buffer = new byte[chunkSize];
			int bytesRead;

			while ((bytesRead = await fileStream.ReadAsync(buffer)) > 0)
			{
				var content = new MultipartFormDataContent("---------------x");
				var x = new ByteArrayContent(buffer.Take(bytesRead).ToArray());
				x.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
				{
					Name = parameterName,
					FileName = fileName
				};
				content.Add(x);
				var requestMessage = CreateRequest(
					HttpMethod.Post,
					fullUrl,
					content: content);

				var response = await _httpClient.SendAsync(requestMessage);
				if (!response.IsSuccessStatusCode)
					throw new HttpClientServiceException(StringConstants.BaseHttpClientServiceExceptionMessage);
			}
		}

		/// <summary>
		/// Create HttpRequestMessage. Add needed authorization header for MoveIt Transfer.
		/// </summary>
		/// <param name="httpMethod">Desired http method.</param>
		/// <param name="url">Full url for the request.</param>
		/// <param name="additionalHeaders">Additional headers except the authorization one.</param>
		/// <param name="content">Content of the request.</param>
		/// <returns><b>HttpRequestMessage</b></returns>
		private HttpRequestMessage CreateRequest(
			HttpMethod httpMethod,
			string url,
			Dictionary<string, string>? additionalHeaders = null,
			HttpContent? content = null)
		{
			var request = new HttpRequestMessage(httpMethod, url);

			var token = _tokenManager.GetValue(StringConstants.MemoryCacheAuthTokenKey);
			if (token != null)
			{
				request.Headers.Add("Authorization", string.Format(StringConstants.AuthorizationHeader, token));
			}

			if (additionalHeaders != null)
			{
				foreach (var header in additionalHeaders)
				{
					request.Headers.Add(header.Key, header.Value);
				}
			}

			if (content != null)
			{
				request.Content = content;
			}

			return request;
		}

		/// <summary>
		/// Send request async.
		/// </summary>
		/// <param name="request">Request message.</param>
		/// <returns>N/A</returns>
		/// <exception cref="HttpClientServiceException"></exception>
		private async System.Threading.Tasks.Task SendRequestAsync(HttpRequestMessage request)
		{
			using var response = await _httpClient.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				throw new HttpClientServiceException(StringConstants.BaseHttpClientServiceExceptionMessage);
			}
		}

		/// <summary>
		/// Send request and deserialize object.
		/// </summary>
		/// <typeparam name="T">Returned object result.</typeparam>
		/// <param name="request">Request message</param>
		/// <returns><b><typeparamref name="T"/></b></returns>
		/// <exception cref="HttpClientServiceException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task<T> SendRequestAsync<T>(HttpRequestMessage request)
		{
			using var response = await _httpClient.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				throw new HttpClientServiceException(StringConstants.BaseHttpClientServiceExceptionMessage);
			}

			return await DeserializeResponse<T>(response);
		}

		private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
		{
			var responseStringData = await response.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<T>(responseStringData)
				   ?? throw new InvalidOperationException(StringConstants.UnsucessfulDeserialization);
		}

		private static async Task<string> GenerateDigest(string filePath)
		{
			await using var stream = File.OpenRead(filePath);
			using var sha256 = SHA256.Create();
			var hashBytes = await sha256.ComputeHashAsync(stream);
			return Convert.ToBase64String(hashBytes);
		}
	}
}
