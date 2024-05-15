using System.Net;

namespace MoveIT.Transfer.Task.Application.Domain.Exceptions
{
	public record ExceptionResponse(HttpStatusCode StatusCode, string Description);
}
