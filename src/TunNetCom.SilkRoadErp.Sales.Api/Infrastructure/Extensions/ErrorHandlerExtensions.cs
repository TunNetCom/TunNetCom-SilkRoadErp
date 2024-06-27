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

                if (exception != null)
                {
                    var globalExceptionHandler = context.RequestServices.GetRequiredService<GlobalExceptionHandler>();
                    await globalExceptionHandler.HandleExceptionAsync(context, exception);
                }
            });
        });
    }
}
