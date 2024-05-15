namespace MoveIT.Transfer.Task.Application.Domain.Exceptions
{
	public class NonExistingTokenException : Exception
	{
		public NonExistingTokenException() { }

		public NonExistingTokenException(string message)
			: base(message) { }

		public NonExistingTokenException(string message, Exception inner)
			: base(message, inner) { }
	}
}
