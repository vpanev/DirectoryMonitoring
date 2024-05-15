namespace MoveIT.Transfer.Task.Application.Domain.Services.HttpClientService
{
	public interface IHttpClientService
	{
		public Task<T> GetAsync<T>(
			string url,
			Dictionary<string, string>? headers = null);

		public Task<T> PostAsync<T>(string url, IEnumerable<KeyValuePair<string, string>> payload,
			Dictionary<string, string>? headers = null);

		public Task<T> PatchAsync<T>(string url, IEnumerable<KeyValuePair<string, string>> payload,
			Dictionary<string, string>? headers = null);

		public System.Threading.Tasks.Task DeleteAsync(string url, Dictionary<string, string>? headers = null);

		public System.Threading.Tasks.Task PostFileAsync(string url, string filePath,
			string fileName, string parameterName = "file");
	}
}
