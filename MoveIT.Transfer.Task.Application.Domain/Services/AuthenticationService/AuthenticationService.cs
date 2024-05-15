using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MoveIT.Transfer.Task.Application.Domain.Exceptions;
using MoveIT.Transfer.Task.Application.Domain.Helpers;
using MoveIT.Transfer.Task.Application.Domain.Helpers.TokenManager;
using MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService.Models;
using MoveIT.Transfer.Task.Application.Domain.Services.HttpClientService;

namespace MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IHttpClientService _httpClientService;
		private readonly IOptions<AppSettings> _appSettings;
		private readonly ITokenManager _tokenManager;
		private readonly IMemoryCache _memoryCache;

		public AuthenticationService(
			IHttpClientService httpClientService,
			IOptions<AppSettings> appSettings,
			ITokenManager tokenManager,
			IMemoryCache memoryCache)
		{
			_httpClientService = httpClientService;
			_appSettings = appSettings;
			_tokenManager = tokenManager;
			_memoryCache = memoryCache;
		}

		/// <summary>
		/// Authenticate.
		/// </summary>
		/// <param name="username">User's username.</param>
		/// <param name="encryptedPassword">User's encrypted password.</param>
		/// <returns><b>GetTokenResponse</b></returns>
		public async Task<GetTokenResponse> Authenticate(string username, string encryptedPassword)
		{
			var decryptedPassword = await EncryptorHelper.DecryptStringAsync(_appSettings.Value.SALT_KEY, encryptedPassword);

			var payload = new Dictionary<string, string>()
			{
				{ "grant_type", "password" },
				{ "username", username },
				{ "password", decryptedPassword },
			};

			var response = await _httpClientService.PostAsync<GetTokenResponse>(StringConstants.GetTokenEndpoint, payload);

			_tokenManager.StoreAuthenticationTokenAndRenewIt(
				response.AccessToken,
				response.RefreshToken,
				response.Expiration,
				async () => await RefreshToken());

			_tokenManager.StoreInCache(StringConstants.MemoryCacheUsernameKey, username);

			return response;
		}

		public async Task<GetTokenResponse> RefreshToken()
		{
			if (!_memoryCache.TryGetValue(StringConstants.MemoryCacheAuthRefreshTokenKey, out string refreshToken))
			{
				throw new NonExistingTokenException(StringConstants.TokenNotFoundMessage);
			}

			var payload = new Dictionary<string, string>()
			{
				{ "grant_type", "refresh_token" },
				{ "refresh_token", refreshToken }
			};

			var response = await _httpClientService.PostAsync<GetTokenResponse>(StringConstants.GetTokenEndpoint, payload);

			return response;
		}
	}
}
