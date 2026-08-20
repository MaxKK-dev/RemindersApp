namespace NotesReminders.Desktop.Common;

public enum ApiError
{
    None,
    Connection,
    Unauthorized,
    Validation,
    NotFound,
    Server,
    Unknown
}

public class ApiResult<T>
{
    private ApiResult()
    {
    }

    public bool IsSuccess { get; private init; }

    public ApiError Error { get; private init; } = ApiError.None;

    public string? ErrorMessage { get; private init; }

    public T? Data { get; private init; }

    public static ApiResult<T> Success(T data)
    {
        return new ApiResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static ApiResult<T> Failure(
        ApiError error,
        string? message = null)
    {
        return new ApiResult<T>
        {
            IsSuccess = false,
            Error = error,
            ErrorMessage = message
        };
    }
    public static ApiResult<T> ConnectionFailure()
    {
        return Failure(
            ApiError.Connection,
            "Unable to connect to the server.");
    }
}