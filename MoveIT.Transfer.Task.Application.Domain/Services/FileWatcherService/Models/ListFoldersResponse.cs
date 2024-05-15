using Newtonsoft.Json;

namespace MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService.Models
{
	public class ListFoldersResponse
	{
		[JsonProperty("items")]
		public List<FolderResponse> Folders { get; set; } = null!;
	}

	public class FolderResponse
	{
		public long Id { get; set; }
		public string Name { get; set; } = null!;
	}
}
