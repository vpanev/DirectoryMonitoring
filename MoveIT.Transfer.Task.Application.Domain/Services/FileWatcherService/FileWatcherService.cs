using MoveIT.Transfer.Task.Application.Domain.Helpers;
using MoveIT.Transfer.Task.Application.Domain.Helpers.TokenManager;
using MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService.Models;
using MoveIT.Transfer.Task.Application.Domain.Services.HttpClientService;
using FileNotFoundException = MoveIT.Transfer.Task.Application.Domain.Exceptions.FileNotFoundException;

namespace MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService
{
	public class FileWatcherService : IFileWatcherService
	{
		private readonly FileSystemWatcher _fileWatcher;
		private readonly IHttpClientService _httpClientService;
		private readonly ITokenManager _tokenManager;

		public FileWatcherService(IHttpClientService httpClientService, ITokenManager tokenManager)
		{
			_httpClientService = httpClientService;
			_tokenManager = tokenManager;
			_fileWatcher = new FileSystemWatcher();
		}

		/// <summary>
		/// Start the monitoring of a folder.
		/// </summary>
		/// <param name="path">Directory's path which will be monitored.</param>
		public void StartFileWatcher(string path)
		{
			_fileWatcher.Path = path;
			_fileWatcher.EnableRaisingEvents = true;

			_fileWatcher.Changed += ProcessFile;
			_fileWatcher.Created += ProcessFile;
			_fileWatcher.Deleted += ProcessFile;
			_fileWatcher.Renamed += ProcessFile;
		}

		public void StopFileWatcher(string path)
		{
			_fileWatcher.Path = path;
			_fileWatcher.EnableRaisingEvents = false;
			_fileWatcher.Changed -= ProcessFile;
			_fileWatcher.Created -= ProcessFile;
			_fileWatcher.Deleted -= ProcessFile;
			_fileWatcher.Renamed -= ProcessFile;
		}

		private async void ProcessFile(object sender, FileSystemEventArgs e)
		{
			switch (e.ChangeType)
			{
				case WatcherChangeTypes.Created:
					await UploadFileAsync(e.FullPath, e.Name!);
					break;
				case WatcherChangeTypes.Deleted:
					await DeleteFileAsync(e.Name!);
					break;
				case WatcherChangeTypes.Renamed:
					var renamedArgs = e as RenamedEventArgs;
					await RenameFileAsync(e.Name!, renamedArgs!.OldName!);
					break;
				case WatcherChangeTypes.Changed:
				case WatcherChangeTypes.All:
				default:
					throw new NotSupportedException(StringConstants.NotSupportedMonitoringEvent);
			}
		}

		private async System.Threading.Tasks.Task RenameFileAsync(string newFileName, string oldFileName)
		{
			var userFiles = await _httpClientService.GetAsync<ListFilesResponse>(StringConstants.ListUserFilesEndpoint);

			long? existingFileId = userFiles.Files
				.Where(x => x.Name.Equals(oldFileName))
				.Select(x => x.Id)
				.FirstOrDefault();

			if (existingFileId == null)
			{
				throw new FileNotFoundException(string.Format(StringConstants.NotFoundFileInMoveItMessage, oldFileName));
			}

			var payload = new Dictionary<string, string>()
			{
				{ "isNew", "false" },
				{ "name", newFileName }
			};

			await _httpClientService.PatchAsync<RenameFileResponse>(
				string.Format(StringConstants.ManipulateUserFileEndpoint, existingFileId),
				payload);
		}

		private async System.Threading.Tasks.Task UploadFileAsync(string filePath, string fileName)
		{
			var username = _tokenManager.GetValue(StringConstants.MemoryCacheUsernameKey);

			var userFolders = await _httpClientService.GetAsync<ListFoldersResponse>(StringConstants.ListUserFoldersEndpoint);

			var mainUserFolderId = userFolders.Folders
				.Where(x => x.Name.Equals(username))
				.Select(x => x.Id)
				.FirstOrDefault();

			await _httpClientService.PostFileAsync(
				string.Format(StringConstants.UploadFileIntoFolderEndpoint, mainUserFolderId),
				filePath,
				fileName);
		}

		private async System.Threading.Tasks.Task DeleteFileAsync(string fileName)
		{
			var userFiles = await _httpClientService.GetAsync<ListFilesResponse>(StringConstants.ListUserFilesEndpoint);

			long? existingFileId = userFiles.Files
				.Where(x => x.Name.Equals(fileName))
				.Select(x => x.Id)
				.FirstOrDefault();

			if (existingFileId == null)
			{
				throw new FileNotFoundException(string.Format(StringConstants.NotFoundFileInMoveItMessage, fileName));
			}

			await _httpClientService.DeleteAsync(string.Format(StringConstants.ManipulateUserFileEndpoint, existingFileId));
		}
	}
}
