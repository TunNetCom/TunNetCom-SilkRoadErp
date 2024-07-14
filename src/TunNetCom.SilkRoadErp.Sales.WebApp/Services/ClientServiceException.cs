using System.Net;
using JsonException = Newtonsoft.Json.JsonException;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services
{
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

        public IList<ValidationError> GetValidationErrors()
        {
            if (StatusCode == HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace(ErrorContent))
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<ValidationError>>(ErrorContent) ?? new List<ValidationError>();
                }
                catch (JsonException)
                {

                }
            }

            return new List<ValidationError>();
        }
    }

    public class ValidationError
    {
        public string? PropertyName { get; set; }
        public string? ErrorMessage { get; set; }
    }
}