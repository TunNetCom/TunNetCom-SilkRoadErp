using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.UpdateDeliveryCar;

public class UpdateDeliveryCarCommandHandler(
    SalesContext _context,
    ILogger<UpdateDeliveryCarCommandHandler> _logger)
    : IRequestHandler<UpdateDeliveryCarCommand, Result<DeliveryCarResponse>>
{
    public async Task<Result<DeliveryCarResponse>> Handle(UpdateDeliveryCarCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating delivery car with Id {DeliveryCarId}", command.Id);

        var deliveryCar = await _context.DeliveryCar
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (deliveryCar == null)
        {
            return Result.Fail("delivery_car_not_found");
        }

        var matriculeExists = await _context.DeliveryCar
            .AsNoTracking()
            .AnyAsync(c => c.Matricule == command.Matricule && c.Id != command.Id, cancellationToken);

        if (matriculeExists)
        {
            return Result.Fail("delivery_car_matricule_already_exists");
        }

        deliveryCar.Matricule = command.Matricule;
        deliveryCar.Owner = command.Owner;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Delivery car {DeliveryCarId} updated successfully", command.Id);

        return new DeliveryCarResponse
        {
            Id = deliveryCar.Id,
            Matricule = deliveryCar.Matricule,
            Owner = deliveryCar.Owner
        };
    }
}

