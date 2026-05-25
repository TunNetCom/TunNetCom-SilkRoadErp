using TunNetCom.SilkRoadErp.SharedKernel.Events;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.Event.Incomming.OrderEvent;

public record class OrderCreatedEvent(Guid orderId, List<OrderItem> OrderItems) : IEvent;

public record class OrderItem(
    Guid? VariantId,
    string ProductName,
    string ProductSku,
    int Quantity,
    decimal UnitPrice,
    decimal TaxAmount,
    decimal DiscountAmount);
