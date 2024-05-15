using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MoveIT.Transfer.Task.Application.Domain.Exceptions;
using MoveIT.Transfer.Task.Application.Domain.Middleware;
using System.Net;
using FileNotFoundException = MoveIT.Transfer.Task.Application.Domain.Exceptions.FileNotFoundException;

namespace MoveIT.Transfer.Task.Application.Domain.UnitTests.Middleware
{
	public class ExceptionHandlerMiddlewareTests
	{
		public Mock<ILogger<ExceptionHandlerMiddleware>> LoggerMock { get; set; } = new();
		public Mock<RequestDelegate> RequestDelegateMock { get; set; } = new();
		public ExceptionHandlerMiddleware ExceptionHandlerMiddleware { get; set; }
		public DefaultHttpContext HttpContext { get; set; }

		public ExceptionHandlerMiddlewareTests()
		{
			ExceptionHandlerMiddleware = new ExceptionHandlerMiddleware(LoggerMock.Object, RequestDelegateMock.Object);
			HttpContext = new DefaultHttpContext();
		}

		[Fact]
		public async System.Threading.Tasks.Task InvokeAsync_WithHttpClientServiceException_ReturnsBadRequest()
		{
			var exception = new HttpClientServiceException("Error message");

			RequestDelegateMock.Setup(rd => rd(HttpContext)).ThrowsAsync(exception);

			await ExceptionHandlerMiddleware.InvokeAsync(HttpContext);

			Assert.Equal((int)HttpStatusCode.BadRequest, HttpContext.Response.StatusCode);
		}

		[Fact]
		public async System.Threading.Tasks.Task InvokeAsync_WithFileNotFoundException_ReturnsBadRequest()
		{
			var exception = new FileNotFoundException("File not found");

			RequestDelegateMock.Setup(rd => rd(HttpContext)).ThrowsAsync(exception);

			await ExceptionHandlerMiddleware.InvokeAsync(HttpContext);

			Assert.Equal((int)HttpStatusCode.BadRequest, HttpContext.Response.StatusCode);
		}
	}
}
