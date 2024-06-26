namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;

public static class ErrorHandlerExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                var logger = context.RequestServices.GetRequiredService<ILogger<GlobalExceptionHandler>>();
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
                await context.Response.WriteAsync(json);
            });
        });
    }
}
