using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetDernieresInfosAchat;

public class GetDernieresInfosAchatQueryHandler(
    SalesContext _context,
    ILogger<GetDernieresInfosAchatQueryHandler> _logger)
    : IRequestHandler<GetDernieresInfosAchatQuery, Result<GetDernieresInfosAchatResponse>>
{
    public async Task<Result<GetDernieresInfosAchatResponse>> Handle(GetDernieresInfosAchatQuery query, CancellationToken cancellationToken)
    {
        // Récupérer le produit
        var produit = await _context.Produit
            .FirstOrDefaultAsync(p => p.Refe == query.RefProduit, cancellationToken);

        if (produit == null)
        {
            _logger.LogWarning("Product {RefProduit} not found", query.RefProduit);
            return Result.Fail("product_not_found");
        }

        // Récupérer la dernière ligne de bon de réception pour ce produit (en ignorant le global query filter)
        var dernierAchat = await _context.LigneBonReception
            .IgnoreQueryFilters()
            .Include(l => l.NumBonRecNavigation)
            .Where(l => l.RefProduit == query.RefProduit)
            .OrderByDescending(l => l.NumBonRecNavigation.Date)
            .FirstOrDefaultAsync(cancellationToken);

        // Si aucune ligne BR trouvée, utiliser les valeurs du produit comme fallback
        if (dernierAchat == null)
        {
            _logger.LogInformation("No receipt note found for product {RefProduit}, using product values", query.RefProduit);
            return Result.Ok(new GetDernieresInfosAchatResponse
            {
                PrixHt = produit.PrixAchat,
                Remise = produit.RemiseAchat,
                Tva = produit.Tva
            });
        }

        _logger.LogInformation("Dernières infos d'achat pour {RefProduit}: Prix={Prix}, Remise={Remise}, TVA={Tva}", 
            query.RefProduit, dernierAchat.PrixHt, dernierAchat.Remise, dernierAchat.Tva);

        return Result.Ok(new GetDernieresInfosAchatResponse
        {
            PrixHt = dernierAchat.PrixHt,
            Remise = dernierAchat.Remise,
            Tva = dernierAchat.Tva
        });
    }
}

