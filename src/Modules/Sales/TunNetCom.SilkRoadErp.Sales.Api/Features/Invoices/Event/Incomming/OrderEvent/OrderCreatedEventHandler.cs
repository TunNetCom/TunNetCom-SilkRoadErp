using TunNetCom.SilkRoadErp.SharedKernel.Events;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.Event.Incomming.OrderEvent;

public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger) : IEventHandler<OrderCreatedEvent>
{
    public Task HandleAsync(
        OrderCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received OrderCreatedEvent. Event details: {@Event} , CancellationToken: {@CancellationToken}", @event, cancellationToken);
        return Task.CompletedTask;
    }
}
