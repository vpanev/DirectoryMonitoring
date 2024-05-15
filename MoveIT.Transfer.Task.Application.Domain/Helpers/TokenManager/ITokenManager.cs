using MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService.Models;

namespace MoveIT.Transfer.Task.Application.Domain.Helpers.TokenManager
{
	public interface ITokenManager
	{
		public void StoreInCache(string key, string value, int expirationInSeconds = int.MaxValue);

		public string? GetValue(string key);

		public void StoreAuthenticationTokenAndRenewIt(
			string token,
			string refreshToken,
			int expiration,
			Func<Task<GetTokenResponse>> refreshTokenFunc);
	}
}
