namespace MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService
{
	public interface IFileWatcherService
	{
		public void StartFileWatcher(string path);
		public void StopFileWatcher(string path);
	}
}
