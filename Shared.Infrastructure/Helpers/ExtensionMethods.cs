using Shared.Infrastructure.Models;

namespace Shared.Infrastructure.Helpers;

public static class ExtensionMethods
{
    public static ApiResponse<T> ToApiResponse<T>(
        this T body,
        string traceId,
        string responseCode,
        string responseMessage)
    {
        return new ApiResponse<T>
        {
            ResponseCode = responseCode,
            ResponseMessage = responseMessage,
            TraceId = traceId,
            Body = body
        };
    }

    public static ApiResponse<T> ToSuccessResponse<T>(
        this T body, string traceId, string message = "Success")
        => body.ToApiResponse(traceId, "00", message);

    public static ApiResponse<T> ToErrorResponse<T>(
        this T body, string traceId, string message, string code = "99")
        => body.ToApiResponse(traceId, code, message);
}
