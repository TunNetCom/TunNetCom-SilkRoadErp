using Dapr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace TunNetCom.SilkRoadErp.SharedKernel.Events.Extensions;

public static class DaprSubscriptionExtensions
{
    public static void MapDaprSubscription<TEvent, THandler>(
        this IEndpointRouteBuilder app,
        string pubsubName)
        where TEvent : class, IEvent
        where THandler : class, IEventHandler<TEvent>
    {
        _ = app.Map($"/{typeof(TEvent).Name}", async context =>
        {
            if (context.Request.Method != "POST")
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                return;
            }

            var logger = context.RequestServices.GetRequiredService<ILogger<THandler>>();


            try
            {
                var cloudEvent = await context.Request.ReadFromJsonAsync<CloudEvent<TEvent>>();
                if (cloudEvent == null || cloudEvent.Data == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid event payload");
                    return;
                }

                var @event = cloudEvent.Data;

                if (@event == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid event payload");
                    return;
                }

                var handler = context.RequestServices.GetRequiredService<IEventHandler<TEvent>>();

                await handler.HandleAsync(@event, default);
                logger.LogInformation(
                    "Processed event {EventName}.",
                    typeof(TEvent).Name
                );
                context.Response.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing event {EventName}", typeof(TEvent).Name);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Error processing event");
            }
        })
        .WithTopic(pubsubName, typeof(TEvent).Name);
    }
}