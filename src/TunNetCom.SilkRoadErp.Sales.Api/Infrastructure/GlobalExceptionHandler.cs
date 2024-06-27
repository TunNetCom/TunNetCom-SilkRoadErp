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

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = exception?.Message,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problemDetails.Status.Value;

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json, cancellationToken);

        return true;
    }
}
