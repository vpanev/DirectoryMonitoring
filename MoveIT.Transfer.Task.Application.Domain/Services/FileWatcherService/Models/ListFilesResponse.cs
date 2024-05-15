using Newtonsoft.Json;

namespace MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService.Models
{
	public class ListFilesResponse
	{
		[JsonProperty("items")]
		public List<FilesResponse> Files { get; set; } = null!;
	}

	public class FilesResponse
	{
		public long Id { get; set; }
		public string Name { get; set; } = null!;
	}
}
