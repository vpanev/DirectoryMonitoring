using Microsoft.Extensions.Caching.Memory;
using MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService.Models;

namespace MoveIT.Transfer.Task.Application.Domain.Helpers.TokenManager
{
	public class TokenManager : ITokenManager
	{
		private readonly IMemoryCache _memoryCache;
		private Timer _refreshTimer = null!;

		public TokenManager(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		/// <summary>
		/// Store a value in memory cache.
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="value">Value</param>
		/// <param name="expirationInSeconds">Expiration in seconds. Default - int.MaxValue</param>
		public void StoreInCache(string key, string value, int expirationInSeconds = int.MaxValue)
		{
			_memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(expirationInSeconds)));
		}

		/// <summary>
		/// Get value from cache by provided key.
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>Value of the entry or null. </returns>
		public string? GetValue(string key)
		{
			return _memoryCache.TryGetValue(key, out string value)
				? value
				: null;
		}

		/// <summary>
		/// Store authentication token and start a timer for renewing it in a period of time.
		/// </summary>
		/// <param name="token">Authentication token.</param>
		/// <param name="refreshToken">Refresh authentication token.</param>
		/// <param name="expiration">Expiration of the token in seconds.</param>
		/// <param name="refreshTokenFunc">Function which will be triggered after the time passes.</param>
		public void StoreAuthenticationTokenAndRenewIt(string token, string refreshToken, int expiration, Func<Task<GetTokenResponse>> refreshTokenFunc)
		{
			StoreAuthenticationTokens(token, refreshToken, expiration);
			StartTokenRefreshTimer(expiration, refreshTokenFunc);
		}

		private void StoreAuthenticationTokens(string token, string refreshToken, int expiration)
		{
			StoreInCache(StringConstants.MemoryCacheAuthTokenKey, token, expiration);
			StoreInCache(StringConstants.MemoryCacheAuthRefreshTokenKey, refreshToken, expiration);
		}

		private void StartTokenRefreshTimer(int expiration, Func<Task<GetTokenResponse>> refreshTokenFunc)
		{
			_refreshTimer = new Timer(async _ => await OnRefreshTimerElapsed(refreshTokenFunc),
				null,
				TimeSpan.FromSeconds(expiration) - TimeSpan.FromMinutes(1),
				TimeSpan.FromSeconds(expiration) - TimeSpan.FromMinutes(1));
		}

		private async System.Threading.Tasks.Task OnRefreshTimerElapsed(Func<Task<GetTokenResponse>> refreshTokenFunc)
		{
			var response = await refreshTokenFunc.Invoke();
			StoreAuthenticationTokens(response.AccessToken, response.RefreshToken, response.Expiration);
			await _refreshTimer.DisposeAsync();
		}
	}
}
