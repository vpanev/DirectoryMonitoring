namespace MoveIT.Transfer.Task.Application.Domain.Helpers
{
	public class StringConstants
	{
		public const string MemoryCacheUsernameKey = "username";
		public const string MemoryCacheAuthTokenKey = "token";
		public const string MemoryCacheAuthRefreshTokenKey = "refresh-token";
		public const string AuthorizationHeader = "Bearer {0}";

		#region Exception messages constants

		public const string NotSupportedMonitoringEvent = "Not supported monitoring event.";
		public const string NotFoundFileInMoveItMessage = "File with name: {0} was not found.";
		public const string BaseHttpClientServiceExceptionMessage = "HTTP client service exception.";
		public const string TokenNotFoundMessage = "No such token. Please log in again!";
		public const string UnsucessfulDeserialization = "Error while trying to deserialize response content.";

		#endregion

		#region MoveIt endpoints

		public const string GetTokenEndpoint = "token";
		public const string ListUserFilesEndpoint = "files";
		public const string ListUserFoldersEndpoint = "folders";
		public const string ManipulateUserFileEndpoint = "files/{0}";
		public const string UploadFileIntoFolderEndpoint = "folders/{0}/files";

		#endregion
	}
}
