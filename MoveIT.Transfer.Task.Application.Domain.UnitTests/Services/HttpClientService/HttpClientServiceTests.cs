using Moq.Protected;
using MoveIT.Transfer.Task.Application.Domain.Exceptions;
using MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService.Models;
using System.Net;

namespace MoveIT.Transfer.Task.Application.Domain.UnitTests.Services.HttpClientService
{
	public class HttpClientServiceTests : TestBase
	{
		[Fact]
		public async System.Threading.Tasks.Task GetAsync_Should_Return_Response_When_Request_Succeeds()
		{
			var expectedResponse = new ListFilesResponse()
			{
				Files = new List<FilesResponse>()
				{
					new()
					{
						Id = 1,
						Name = "FakeFile"
					}
				}
			};

			HttpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
				});

			var response = await HttpClientService.GetAsync<ListFilesResponse>("");

			Assert.Equal(expectedResponse.Files.First().Name, response.Files.First().Name);
		}

		[Fact]
		public async System.Threading.Tasks.Task GetAsync_Should_Throw_When_Response_Is_Not_Okay()
		{
			var expectedResponse = new ListFilesResponse()
			{
				Files = new List<FilesResponse>()
				{
					new()
					{
						Id = 1,
						Name = "FakeFile"
					}
				}
			};

			HttpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest,
					Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
				});

			await Assert.ThrowsAsync<HttpClientServiceException>(() => HttpClientService.GetAsync<ListFilesResponse>(""));
		}

		[Fact]
		public async System.Threading.Tasks.Task PostAsync_Should_Return_Response_When_Request_Succeeds()
		{
			var expectedResponse = new GetTokenResponse()
			{
				AccessToken = "FakeAccessToken",
				Expiration = 60,
				RefreshToken = "FakeRefreshToken",
				TokenType = "fake_token-type"
			};
			var payload = new List<KeyValuePair<string, string>>();

			HttpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
				});

			var response = await HttpClientService.PostAsync<GetTokenResponse>("", payload);

			Assert.Equal(expectedResponse.AccessToken, response.AccessToken);
		}

		[Fact]
		public async System.Threading.Tasks.Task PostAsync_Should_Throw_When_Response_Is_Not_Okay()
		{
			var expectedResponse = new GetTokenResponse()
			{
				AccessToken = "FakeAccessToken",
				Expiration = 60,
				RefreshToken = "FakeRefreshToken",
				TokenType = "fake_token-type"
			};

			HttpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
				});

			// Assert
			await Assert.ThrowsAsync<HttpClientServiceException>(() => HttpClientService.GetAsync<ListFilesResponse>(""));
		}

		[Fact]
		public async System.Threading.Tasks.Task PatchAsync_Should_Return_Response_When_Request_Succeeds()
		{
			var expectedResponse = new RenameFileResponse()
			{
				Id = "1",
				Name = "NewName",
				OriginalFileName = "OldName"
			};
			var payload = new List<KeyValuePair<string, string>>();

			HttpMessageHandlerMock
				.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
				});

			var response = await HttpClientService.PatchAsync<RenameFileResponse>("", payload);

			Assert.Equal(expectedResponse.Name, response.Name);
			Assert.Equal(expectedResponse.OriginalFileName, response.OriginalFileName);
		}
	}
}
