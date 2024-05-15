namespace MoveIT.Transfer.Task.Application.Domain.UnitTests.Helpers
{
	public class TokenManagerTests : TestBase
	{
		public TokenManager TokenManager { get; set; }

		public TokenManagerTests()
		{
			TokenManager = new TokenManager(MemoryCache);
		}

		[Fact]
		public void StoreToken_CorrectlyStoresToken()
		{
			var expectedValue = "value";

			TokenManager.StoreInCache("key", "value");

			Assert.Equal(expectedValue, MemoryCache.Get<string>("key"));
		}

		[Fact]
		public void GetToken_WhenKeyDoesNotExist_ReturnsNull()
		{
			var nonExistentKey = "non-existent";

			var token = TokenManager.GetValue(nonExistentKey);

			Assert.Null(token);
		}
	}
}
