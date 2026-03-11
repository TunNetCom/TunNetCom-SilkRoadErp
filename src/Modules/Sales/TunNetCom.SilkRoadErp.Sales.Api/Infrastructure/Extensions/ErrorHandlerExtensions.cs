namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;

public static class ErrorHandlerExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        _ = app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                if (exception != null)
                {
                    var globalExceptionHandler = context.RequestServices.GetRequiredService<GlobalExceptionHandler>();
                    _ = await globalExceptionHandler.HandleExceptionAsync(context, exception);
                }
            });
        });
    }
}
