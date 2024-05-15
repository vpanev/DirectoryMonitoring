namespace MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService.Models
{
	public class GetTokenRequest
	{
		public string Username { get; set; } = null!;
		public string Password { get; set; } = null!;
	}
}
