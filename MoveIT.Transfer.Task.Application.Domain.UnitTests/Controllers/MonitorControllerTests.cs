using Microsoft.AspNetCore.Mvc;
using MoveIT.Transfer.Task.Application.Domain.Controllers;
using MoveIT.Transfer.Task.Application.Domain.Services.FileWatcherService;
using System.Text;

namespace MoveIT.Transfer.Task.Application.Domain.UnitTests.Controllers
{
	public class MonitorControllerTests : TestBase
	{
		private readonly Mock<IFileWatcherService> _fileWatcherServiceMock = new();
		private MonitorController MonitorController { get; }

		public MonitorControllerTests()
		{
			MonitorController = new MonitorController(_fileWatcherServiceMock.Object);
		}

		[Fact]
		public void SetMonitorFolder_ValidFolderPath_ReturnsOk()
		{
			var folderPath = @"c:\example\folder";

			var result = MonitorController
				.SetMonitorFolder(Convert.ToBase64String(Encoding.UTF8.GetBytes(folderPath))) as OkResult;

			Assert.NotNull(result);
			Assert.Equal(200, result.StatusCode);
			_fileWatcherServiceMock.Verify(m => m.StartFileWatcher(folderPath), Times.Once);
		}

		[Fact]
		public void SetMonitorFolder_InvalidFolderPath_ReturnsBadRequest()
		{
			var folderPath = "";

			var result = MonitorController.SetMonitorFolder(folderPath) as BadRequestObjectResult;

			Assert.NotNull(result);
			Assert.Equal(400, result.StatusCode);
			Assert.Equal("Folder path is required.", result.Value);
			_fileWatcherServiceMock.Verify(m => m.StartFileWatcher(It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void StopMonitorFolder_ValidFolderPath_ReturnsOk()
		{
			var folderPath = @"c:\example\folder";

			var result = MonitorController.StopMonitorFolder(Convert.ToBase64String(Encoding.UTF8.GetBytes(folderPath))) as OkResult;

			Assert.NotNull(result);
			Assert.Equal(200, result.StatusCode);
			_fileWatcherServiceMock.Verify(m => m.StopFileWatcher(folderPath), Times.Once);
		}

		[Fact]
		public void StopMonitorFolder_InvalidFolderPath_ReturnsBadRequest()
		{
			var folderPath = "";

			var result = MonitorController.StopMonitorFolder(folderPath) as BadRequestObjectResult;

			Assert.NotNull(result);
			Assert.Equal(400, result.StatusCode);
			Assert.Equal("Folder path is required.", result.Value);
			_fileWatcherServiceMock.Verify(m => m.StopFileWatcher(It.IsAny<string>()), Times.Never);
		}
	}
}
