public class Result<T>
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public int StatusCode { get; init; }
    public T? Data { get; init; }

    private Result(bool isSuccess, T? data, string message, int statusCode)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T data, int statusCode = 200, string message = "Success")
        => new(true, data, message, statusCode);

    public static Result<T> Fail(string message, int statusCode)
        => new(false, default, message, statusCode);
}
