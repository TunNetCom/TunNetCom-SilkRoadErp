using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetDernierPrixAchat;

public class GetDernierPrixAchatQueryHandler(
    SalesContext _context,
    ILogger<GetDernierPrixAchatQueryHandler> _logger)
    : IRequestHandler<GetDernierPrixAchatQuery, Result<decimal>>
{
    public async Task<Result<decimal>> Handle(GetDernierPrixAchatQuery query, CancellationToken cancellationToken)
    {
        // Récupérer le produit
        var produit = await _context.Produit
            .FirstOrDefaultAsync(p => p.Refe == query.RefProduit, cancellationToken);

        if (produit == null)
        {
            _logger.LogWarning("Product {RefProduit} not found", query.RefProduit);
            return Result.Fail("product_not_found");
        }

        // Récupérer le dernier prix d'achat depuis LigneBonReception (en ignorant le global query filter)
        var dernierAchat = await _context.LigneBonReception
            .IgnoreQueryFilters()
            .Include(l => l.NumBonRecNavigation)
            .Where(l => l.RefProduit == query.RefProduit)
            .OrderByDescending(l => l.NumBonRecNavigation.Date)
            .Select(l => l.PrixHt)
            .FirstOrDefaultAsync(cancellationToken);

        // Si aucun achat trouvé, retourner le prix d'achat du produit
        var dernierPrixAchat = dernierAchat != 0 ? dernierAchat : produit.PrixAchat;

        _logger.LogInformation("Dernier prix d'achat pour {RefProduit}: {Prix}", query.RefProduit, dernierPrixAchat);
        return Result.Ok(dernierPrixAchat);
    }
}

