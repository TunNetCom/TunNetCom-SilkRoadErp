using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.GetSousFamilleProduits;

public class GetSousFamilleProduitsQueryHandler(
    SalesContext _context,
    ILogger<GetSousFamilleProduitsQueryHandler> _logger)
    : IRequestHandler<GetSousFamilleProduitsQuery, List<SousFamilleProduitResponse>>
{
    public async Task<List<SousFamilleProduitResponse>> Handle(GetSousFamilleProduitsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching product subfamilies");
        
        var sousFamillesQuery = _context.SousFamilleProduit
            .Include(sf => sf.FamilleProduit)
            .AsQueryable();

        if (query.FamilleProduitId.HasValue)
        {
            sousFamillesQuery = sousFamillesQuery.Where(sf => sf.FamilleProduitId == query.FamilleProduitId.Value);
        }

        var sousFamilles = await sousFamillesQuery
            .OrderBy(sf => sf.FamilleProduit.Nom)
            .ThenBy(sf => sf.Nom)
            .Select(sf => new SousFamilleProduitResponse
            {
                Id = sf.Id,
                Nom = sf.Nom,
                FamilleProduitId = sf.FamilleProduitId,
                FamilleProduitNom = sf.FamilleProduit.Nom
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Fetched {Count} product subfamilies", sousFamilles.Count);
        return sousFamilles;
    }
}

