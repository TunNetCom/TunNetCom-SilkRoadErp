using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductFamilies.GetFamilleProduits;

public class GetFamilleProduitsQueryHandler(
    SalesContext _context,
    ILogger<GetFamilleProduitsQueryHandler> _logger)
    : IRequestHandler<GetFamilleProduitsQuery, List<FamilleProduitResponse>>
{
    public async Task<List<FamilleProduitResponse>> Handle(GetFamilleProduitsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all product families");
        
        var familles = await _context.FamilleProduit
            .OrderBy(f => f.Nom)
            .Select(f => new FamilleProduitResponse
            {
                Id = f.Id,
                Nom = f.Nom
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Fetched {Count} product families", familles.Count);
        return familles;
    }
}

