using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.CreateDeliveryCar;

public class CreateDeliveryCarCommandHandler(
    SalesContext _context,
    ILogger<CreateDeliveryCarCommandHandler> _logger)
    : IRequestHandler<CreateDeliveryCarCommand, Result<DeliveryCarResponse>>
{
    public async Task<Result<DeliveryCarResponse>> Handle(CreateDeliveryCarCommand command, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(Domain.Entites.DeliveryCar), command);

        var carExists = await _context.DeliveryCar
            .AsNoTracking()
            .AnyAsync(c => c.Matricule == command.Matricule, cancellationToken);

        if (carExists)
        {
            return Result.Fail("delivery_car_matricule_already_exists");
        }

        var deliveryCar = new Domain.Entites.DeliveryCar
        {
            Matricule = command.Matricule,
            Owner = command.Owner
        };

        _context.DeliveryCar.Add(deliveryCar);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully(nameof(Domain.Entites.DeliveryCar), deliveryCar.Id);

        return new DeliveryCarResponse
        {
            Id = deliveryCar.Id,
            Matricule = deliveryCar.Matricule,
            Owner = deliveryCar.Owner
        };
    }
}




