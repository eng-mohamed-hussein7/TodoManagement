using System;
namespace Models;
public class ApiResponse<T>
{
    public bool Succeeded { get; set; }
    public int StatusResult { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}
