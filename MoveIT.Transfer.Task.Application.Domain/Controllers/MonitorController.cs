using Microsoft.AspNetCore.Mvc;
using MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService;
using System.Text;

namespace MoveIT.Transfer.Task.Application.Domain.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MonitorController : Controller
	{
		private readonly IFileWatcherService _fileWatcherService;
		public MonitorController(IFileWatcherService fileWatcherService)
		{
			_fileWatcherService = fileWatcherService;
		}

		/// <summary>
		/// Endpoint, used for setting locally the directory which will be monitored.
		/// </summary>
		/// <param name="folderPath">Path to the directory.</param>
		/// <returns></returns>
		[HttpGet("folder/{folderPath}/start")]
		public IActionResult SetMonitorFolder([FromRoute] string folderPath)
		{
			if (string.IsNullOrEmpty(folderPath))
			{
				return BadRequest("Folder path is required.");
			}

			var decodedPath = Convert.FromBase64String(folderPath);
			_fileWatcherService.StartFileWatcher(Encoding.UTF8.GetString(decodedPath));
			return Ok();
		}

		/// <summary>
		/// Endpoint, used for setting locally the directory which will be monitored.
		/// </summary>
		/// <param name="folderPath">Path to the directory.</param>
		/// <returns></returns>
		[HttpGet("folder/{folderPath}/stop")]
		public IActionResult StopMonitorFolder([FromRoute] string folderPath)
		{
			if (string.IsNullOrEmpty(folderPath))
			{
				return BadRequest("Folder path is required.");
			}

			var decodedPath = Convert.FromBase64String(folderPath);
			_fileWatcherService.StopFileWatcher(Encoding.UTF8.GetString(decodedPath));
			return Ok();
		}
	}

}