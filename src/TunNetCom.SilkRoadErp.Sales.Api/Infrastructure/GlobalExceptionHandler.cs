namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        return await HandleExceptionAsync(httpContext, exception, cancellationToken);
    }

    public async Task<bool> HandleExceptionAsync(HttpContext context, Exception exception, CancellationToken cancellationToken = default)
    {
        logger.LogError(exception, "An unhandled exception has occurred while executing the request.");

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .Select(e => new ValidationError { PropertyName = e.PropertyName, ErrorMessage = e.ErrorMessage })
                .ToList();

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation Error",
                Detail = "One or more validation errors occurred.",
                Instance = context.Request.Path,
                Extensions = { { "errors", errors } }
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = problemDetails.Status.Value;

            var json = System.Text.Json.JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json, cancellationToken);

            return true;
        }

        var genericProblemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = exception?.Message,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = genericProblemDetails.Status.Value;

        var genericJson = System.Text.Json.JsonSerializer.Serialize(genericProblemDetails);
        await context.Response.WriteAsync(genericJson, cancellationToken);

        return true;
    }
}
