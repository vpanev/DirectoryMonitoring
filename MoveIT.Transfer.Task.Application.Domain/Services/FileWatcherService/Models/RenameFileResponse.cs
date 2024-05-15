namespace MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService.Models
{
	public class RenameFileResponse
	{
		public string Id { get; set; } = null!;
		public string Name { get; set; } = null!;
		public string OriginalFileName { get; set; } = null!;
	}
}
