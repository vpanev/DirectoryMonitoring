using MoveIT.Transfer.Task.Application.Domain.Exceptions;

namespace MoveIT.Transfer.Task.Application.Domain.UnitTests.Services.AuthenticationService
{
	public class AuthenticationServiceTests : TestBase
	{
		[Fact]
		public async System.Threading.Tasks.Task Authenticate_Should_Decrypt_And_Authenticate()
		{
			var username = "TestUser";
			var password = "DIFJZYbmHawfmzwkWR+FKQ==";

			var response = new GetTokenResponse
			{
				AccessToken = "TestToken",
				RefreshToken = "TestRefreshToken",
				Expiration = 60
			};

			HttpClientServiceMock
				.Setup(x =>
					x.PostAsync<GetTokenResponse>("token", It.IsAny<Dictionary<string, string>>(), null))
				.ReturnsAsync(response);

			var result = await AuthenticationService.Authenticate(username, password);

			Assert.Equal("TestToken", result.AccessToken);
		}

		[Fact]
		public async System.Threading.Tasks.Task Authenticate_Should_Store_Authentication_Token()
		{
			var username = "TestUser";
			var encryptedPassword = "DIFJZYbmHawfmzwkWR+FKQ==";

			var response = new GetTokenResponse
			{
				AccessToken = "TestToken",
				RefreshToken = "TestRefreshToken",
				Expiration = 60
			};

			HttpClientServiceMock
				.Setup(x =>
					x.PostAsync<GetTokenResponse>("token", It.IsAny<Dictionary<string, string>>(), null))
				.ReturnsAsync(response);

			var result = await AuthenticationService.Authenticate(username, encryptedPassword);

			Assert.Equal("TestToken", result.AccessToken);
			TokenManagerMock.Verify(
				x => x.StoreAuthenticationTokenAndRenewIt(
					result.AccessToken,
					result.RefreshToken,
					result.Expiration,
					It.IsAny<Func<Task<GetTokenResponse>>>()),
				Times.Once);
		}

		[Fact]
		public async System.Threading.Tasks.Task RefreshToken_WithValidRefreshToken_ReturnsTokenResponse()
		{
			var refreshToken = "ValidRefreshToken";
			var accessToken = "NewAuthenticationToken";
			var expectedExpiration = DateTime.UtcNow.AddHours(1).Second;

			var response = new GetTokenResponse
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				Expiration = expectedExpiration
			};

			MemoryCache.Set("refresh-token", refreshToken);

			HttpClientServiceMock
				.Setup(x => x.PostAsync<GetTokenResponse>("token", It.IsAny<Dictionary<string, string>>(), null))
				.ReturnsAsync(response);

			var result = await AuthenticationService.RefreshToken();

			Assert.Equal(accessToken, result.AccessToken);
			Assert.Equal(refreshToken, result.RefreshToken);
			Assert.Equal(expectedExpiration, result.Expiration);
		}

		[Fact]
		public async System.Threading.Tasks.Task RefreshToken_InvalidToken_ThrowsException()
		{
			await Assert.ThrowsAsync<NonExistingTokenException>(() => AuthenticationService.RefreshToken());
		}

		[Fact]
		public async System.Threading.Tasks.Task RefreshToken_WithValidRefreshToken_ThrowsExceptionIfHttpClientServiceFails()
		{
			var refreshToken = "ValidRefreshToken";

			MemoryCache.Set("refresh-token", refreshToken);

			HttpClientServiceMock.Setup(x => x.PostAsync<GetTokenResponse>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), null))
				.ThrowsAsync(new Exception("HTTP request failed"));

			await Assert.ThrowsAsync<Exception>(() => AuthenticationService.RefreshToken());
		}
	}
}
