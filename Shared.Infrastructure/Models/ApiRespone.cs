namespace Shared.Infrastructure.Models;

public class ApiResponse<T>
{
    public string ResponseCode { get; set; } = string.Empty;
    public string ResponseMessage { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public T? Body { get; set; }
}