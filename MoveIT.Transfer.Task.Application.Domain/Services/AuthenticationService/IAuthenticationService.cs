using MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService.Models;

namespace MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService
{
	public interface IAuthenticationService
	{
		public Task<GetTokenResponse> Authenticate(string username, string password);
	}
}
