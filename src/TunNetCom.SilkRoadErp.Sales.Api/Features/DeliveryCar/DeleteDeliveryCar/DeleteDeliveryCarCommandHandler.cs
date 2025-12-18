using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.DeleteDeliveryCar;

public class DeleteDeliveryCarCommandHandler(
    SalesContext _context,
    ILogger<DeleteDeliveryCarCommandHandler> _logger)
    : IRequestHandler<DeleteDeliveryCarCommand, Result>
{
    public async Task<Result> Handle(DeleteDeliveryCarCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting delivery car with Id {DeliveryCarId}", command.Id);

        var deliveryCar = await _context.DeliveryCar
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (deliveryCar == null)
        {
            return Result.Fail("delivery_car_not_found");
        }

        _context.DeliveryCar.Remove(deliveryCar);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Delivery car {DeliveryCarId} deleted successfully", command.Id);

        return Result.Ok();
    }
}




