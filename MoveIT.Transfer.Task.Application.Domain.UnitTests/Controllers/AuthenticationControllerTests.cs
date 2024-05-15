using MoveIT.Transfer.Task.Application.Domain.Controllers;

namespace MoveIT.Transfer.Task.Application.Domain.UnitTests.Controllers
{
	public class AuthenticationControllerTests
	{
		private readonly Mock<IAuthenticationService> _authenticationServiceMock = new();
		private AuthenticationController AuthenticationController { get; }

		public AuthenticationControllerTests()
		{
			AuthenticationController = new AuthenticationController(_authenticationServiceMock.Object);
		}

		[Fact]
		public async System.Threading.Tasks.Task Authenticate_ValidCredentials_ReturnsToken()
		{
			var request = new GetTokenRequest
			{
				Username = "username",
				Password = "password"
			};

			const string expectedToken = "test_token";
			_authenticationServiceMock
				.Setup(x => x.Authenticate(request.Username, request.Password))
				.ReturnsAsync(new GetTokenResponse
				{
					AccessToken = expectedToken
				});

			var result = await AuthenticationController.Authenticate(request);

			Assert.NotNull(result);
			Assert.IsType<GetTokenResponse>(result);
			Assert.Equal(expectedToken, result.AccessToken);
		}
	}
}
