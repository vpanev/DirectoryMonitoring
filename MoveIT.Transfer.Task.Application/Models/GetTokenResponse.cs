namespace MoveIT.Transfer.Task.Application.Models
{
	public class GetTokenResponse
	{
		public string AccessToken { get; set; } = null!;

		public string TokenType { get; set; } = null!;

		public int Expiration { get; set; }

		public string RefreshToken { get; set; } = null!;
	}
}
