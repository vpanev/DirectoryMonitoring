using Microsoft.AspNetCore.Mvc;
using MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService;
using MoveIT.Transfer.Task.Application.Domain.Services.AuthenticationService.Models;

namespace MoveIT.Transfer.Task.Application.Domain.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IAuthenticationService _authenticationService;

		public AuthenticationController(IAuthenticationService authenticationService)
		{
			_authenticationService = authenticationService;
		}

		/// <summary>
		/// Authenticate user with provided credentials.
		/// </summary>
		/// <param name="request">Request, which contains user's username and password inside.</param>
		/// <returns>GetTokenResponse</returns>
		[HttpPost]
		public async Task<GetTokenResponse> Authenticate([FromBody] GetTokenRequest request)
		{
			return await _authenticationService.Authenticate(request.Username, request.Password);
		}
	}
}
