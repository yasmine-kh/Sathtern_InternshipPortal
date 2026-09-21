namespace backend.Services;

/// <summary>
/// Outcome of a service operation. Controllers map these onto HTTP status
/// codes (Success -> 200/201, NotFound -> 404, Conflict -> 409, Error -> 400).
/// </summary>
public enum ResultStatus
{
    Success,
    NotFound,
    Conflict,
    Error
}

/// <summary>Result of an operation that returns no value.</summary>
public class ServiceResult
{
    public ResultStatus Status { get; }
    public string? ErrorMessage { get; }
    public bool IsSuccess => Status == ResultStatus.Success;

    protected ServiceResult(ResultStatus status, string? errorMessage)
    {
        Status = status;
        ErrorMessage = errorMessage;
    }

    public static ServiceResult Ok() => new(ResultStatus.Success, null);
    public static ServiceResult NotFound(string message) => new(ResultStatus.NotFound, message);
    public static ServiceResult Conflict(string message) => new(ResultStatus.Conflict, message);
    public static ServiceResult Error(string message) => new(ResultStatus.Error, message);
}

/// <summary>Result of an operation that returns a value on success.</summary>
public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; }

    private ServiceResult(ResultStatus status, T? value, string? errorMessage)
        : base(status, errorMessage)
    {
        Value = value;
    }

    public static ServiceResult<T> Ok(T value) => new(ResultStatus.Success, value, null);
    public static new ServiceResult<T> NotFound(string message) => new(ResultStatus.NotFound, default, message);
    public static new ServiceResult<T> Conflict(string message) => new(ResultStatus.Conflict, default, message);
    public static new ServiceResult<T> Error(string message) => new(ResultStatus.Error, default, message);
}
