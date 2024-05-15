namespace MoveIT.Transfer.Task.Application.Domain.Exceptions
{
	public class HttpClientServiceException : Exception
	{
		public HttpClientServiceException() { }

		public HttpClientServiceException(string message)
			: base(message) { }

		public HttpClientServiceException(string message, Exception inner)
			: base(message, inner) { }
	}
}
