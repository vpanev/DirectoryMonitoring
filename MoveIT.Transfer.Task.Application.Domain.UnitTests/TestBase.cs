using MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService;
using MoveIT.Transfer.Task.Application.Domain.Services.HttpClientService;

namespace MoveIT.Transfer.Task.Application.Domain.UnitTests
{
	public class TestBase
	{
		protected Mock<IHttpClientService> HttpClientServiceMock { get; }
		protected Mock<IOptions<AppSettings>> AppSettingsMock { get; }
		protected Mock<ITokenManager> TokenManagerMock { get; }
		protected Mock<IHttpClientFactory> HttpClientFactoryMock { get; }
		protected Mock<HttpMessageHandler> HttpMessageHandlerMock { get; }
		protected HttpClient HttpClient { get; }
		protected MemoryCache MemoryCache { get; }
		protected AuthenticationService AuthenticationService { get; private set; }
		protected HttpClientService HttpClientService { get; private set; }
		protected FileWatcherService FileWatcherService { get; private set; }

		public TestBase()
		{
			HttpClientServiceMock = new Mock<IHttpClientService>();
			AppSettingsMock = new Mock<IOptions<AppSettings>>();
			TokenManagerMock = new Mock<ITokenManager>();

			// Mock IHttpClientFactory
			HttpClientFactoryMock = new Mock<IHttpClientFactory>();
			HttpMessageHandlerMock = new Mock<HttpMessageHandler>();
			HttpClient = new HttpClient(HttpMessageHandlerMock.Object)
			{
				BaseAddress = new Uri("https://fakeapi.com/fake/test")
			};
			HttpClientFactoryMock
				.Setup(x => x.CreateClient(It.IsAny<string>()))
				.Returns(HttpClient);
			// Mock IHttpClientFactory

			MemoryCache = new MemoryCache(new MemoryCacheOptions());

			var appSettings = new AppSettings { MOVE_IT_API_URL = "API_URL", SALT_KEY = "b14ca5898a4e4133bbce2ea2315a1916" };
			AppSettingsMock.SetupGet(x => x.Value).Returns(appSettings);

			AuthenticationService = new AuthenticationService(
				HttpClientServiceMock.Object,
				AppSettingsMock.Object,
				TokenManagerMock.Object,
				MemoryCache);

			FileWatcherService = new FileWatcherService(HttpClientServiceMock.Object, TokenManagerMock.Object);

			HttpClientService = new HttpClientService(HttpClientFactoryMock.Object, AppSettingsMock.Object, TokenManagerMock.Object);
		}
	}
}
