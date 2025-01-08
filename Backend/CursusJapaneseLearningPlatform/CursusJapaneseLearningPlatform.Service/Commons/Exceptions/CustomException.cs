namespace CursusJapaneseLearningPlatform.Service.Commons.Exceptions;

public class CustomException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }
    public object? ErrorMessage { get; }

    public CustomException(int statusCode, string errorCode, string? message = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ErrorMessage = message ?? "An error occurred";
    }

    public CustomException(int statusCode, string errorCode, object errorMessage)
        : base(errorMessage.ToString())
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
