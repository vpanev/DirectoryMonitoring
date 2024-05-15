using Newtonsoft.Json;

namespace MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService.Models
{
	public class GetTokenResponse
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; set; } = null!;

		[JsonProperty("token_type")]
		public string TokenType { get; set; } = null!;

		[JsonProperty("expires_in")]
		public int Expiration { get; set; }

		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; } = null!;
	}
}
