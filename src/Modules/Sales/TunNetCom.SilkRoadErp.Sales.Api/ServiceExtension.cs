using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.Event.Incomming.OrderEvent;
using TunNetCom.SilkRoadErp.SharedKernel.Events;
using TunNetCom.SilkRoadErp.SharedKernel.Events.Extensions;

namespace TunNetCom.SilkRoadErp.Sales.Api;

public static class ServiceExtension
{
    public static IServiceCollection AddEventHandler(this IServiceCollection services)
    {
        _ = services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
        return services;
    }

    public static WebApplication MapEventEndPoint(this WebApplication app)
    {
        app.MapDaprSubscription<OrderCreatedEvent, OrderCreatedEventHandler>("pubsub");
        return app;
    }
}
