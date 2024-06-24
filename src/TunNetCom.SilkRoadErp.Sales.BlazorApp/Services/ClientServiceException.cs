using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace TunNetCom.SilkRoadErp.Sales.BlazorApp.Services;
public class ClientServiceException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorContent { get; }

    public ClientServiceException(HttpStatusCode statusCode, string errorContent)
        : base($"HTTP request failed with status code {statusCode}")
    {
        StatusCode = statusCode;
        ErrorContent = errorContent;
    }

    public IDictionary<string, string[]> GetValidationErrors()
    {
        if (StatusCode == HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace(ErrorContent))
        {
            var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(ErrorContent);
            return problemDetails?.Errors ?? new Dictionary<string, string[]>();
        }

        return new Dictionary<string, string[]>();
    }
}



