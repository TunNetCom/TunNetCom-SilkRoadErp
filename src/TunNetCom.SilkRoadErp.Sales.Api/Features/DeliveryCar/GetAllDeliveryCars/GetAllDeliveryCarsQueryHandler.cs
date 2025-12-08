using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.GetAllDeliveryCars;

public class GetAllDeliveryCarsQueryHandler(
    SalesContext _context,
    ILogger<GetAllDeliveryCarsQueryHandler> _logger)
    : IRequestHandler<GetAllDeliveryCarsQuery, List<DeliveryCarResponse>>
{
    public async Task<List<DeliveryCarResponse>> Handle(GetAllDeliveryCarsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all delivery cars");

        var deliveryCars = await _context.DeliveryCar
            .AsNoTracking()
            .OrderBy(c => c.Matricule)
            .Select(c => new DeliveryCarResponse
            {
                Id = c.Id,
                Matricule = c.Matricule,
                Owner = c.Owner
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} delivery cars", deliveryCars.Count);

        return deliveryCars;
    }
}



