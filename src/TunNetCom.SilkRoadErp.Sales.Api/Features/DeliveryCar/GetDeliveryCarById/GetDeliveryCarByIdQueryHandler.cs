using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.GetDeliveryCarById;

public class GetDeliveryCarByIdQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryCarByIdQueryHandler> _logger)
    : IRequestHandler<GetDeliveryCarByIdQuery, Result<DeliveryCarResponse>>
{
    public async Task<Result<DeliveryCarResponse>> Handle(GetDeliveryCarByIdQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting delivery car with Id {DeliveryCarId}", query.Id);

        var deliveryCar = await _context.DeliveryCar
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (deliveryCar == null)
        {
            return Result.Fail("delivery_car_not_found");
        }

        return new DeliveryCarResponse
        {
            Id = deliveryCar.Id,
            Matricule = deliveryCar.Matricule,
            Owner = deliveryCar.Owner
        };
    }
}

