using MoveIT.Transfer.Task.Application.Domain.Exceptions;
using System.Net;
using FileNotFoundException = MoveIT.Transfer.Task.Application.Domain.Exceptions.FileNotFoundException;

namespace MoveIT.Transfer.Task.Application.Domain.Middleware
{
	public class ExceptionHandlerMiddleware
	{
		private readonly ILogger<ExceptionHandlerMiddleware> _logger;
		private readonly RequestDelegate _next;

		public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
		{
			_logger = logger;
			_next = next;
		}

		public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private async System.Threading.Tasks.Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			if (context.Response.HasStarted)
			{
				_logger.LogError(context.Request.Method, context.Request.Path, context.Response.StatusCode, exception);
			}
			else
			{
				var exceptionResponse = CreateResponse(exception);
				context.Response.StatusCode = (int)exceptionResponse.StatusCode;
				await context.Response.WriteAsJsonAsync(exceptionResponse);
			}
		}

		private static ExceptionResponse CreateResponse(Exception exception)
		{
			ExceptionResponse response;
			switch (exception)
			{
				case HttpClientServiceException hcse:
					response = new ExceptionResponse(HttpStatusCode.BadRequest, "Error while trying to send a request. " + hcse.Message);
					break;
				case FileNotFoundException fnfe:
					response = new ExceptionResponse(HttpStatusCode.BadRequest, "Error while trying to find a file. " + fnfe.Message);
					break;
				case NonExistingTokenException nete:
					response = new ExceptionResponse(HttpStatusCode.BadRequest, "Error while trying to access authentication token. " + nete.Message);
					break;
				default:
					response = new ExceptionResponse(HttpStatusCode.InternalServerError, "Internal server error " + exception.Message);
					break;
			}

			return response;
		}
	}
}
