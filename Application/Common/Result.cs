using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Common;

public class Result
{
    public bool Succeeded { get; set; }
    public StatusResult StatusResult { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public Error Error { get; set; }


    public Result(bool succeeded, StatusResult statusResult = StatusResult.Success, string? message = null, Error? error = null, object? data = null)
    {
        Succeeded = succeeded;
        StatusResult = statusResult;
        Message = message;
        Data = data;
        Error = error;
    }
    public static Result Success(string? messages = null, Error? error = null, object? data = null)
    {
        return new Result(true, StatusResult.Success, messages, error, data);
    }

    public static Result Failure(StatusResult statusResult = StatusResult.Falid, string? messages = null, Error error = null, object? data = null)
    {
        return new Result(false, statusResult, messages, error, data);
    }
}
public enum StatusResult
{
    Falid,
    Success,
    Exist,
    NotExists,
}
