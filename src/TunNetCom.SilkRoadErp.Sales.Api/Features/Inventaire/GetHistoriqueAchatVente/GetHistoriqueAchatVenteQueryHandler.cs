using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetHistoriqueAchatVente;

public class GetHistoriqueAchatVenteQueryHandler(
    SalesContext _context,
    ILogger<GetHistoriqueAchatVenteQueryHandler> _logger)
    : IRequestHandler<GetHistoriqueAchatVenteQuery, Result<List<HistoriqueAchatVenteResponse>>>
{
    public async Task<Result<List<HistoriqueAchatVenteResponse>>> Handle(GetHistoriqueAchatVenteQuery query, CancellationToken cancellationToken)
    {
        var historique = new List<HistoriqueAchatVenteResponse>();

        // Récupérer l'historique d'achat depuis LigneBonReception (en ignorant le global query filter)
        var achats = await _context.LigneBonReception
            .IgnoreQueryFilters()
            .Include(l => l.NumBonRecNavigation)
                .ThenInclude(br => br.IdFournisseurNavigation)
            .Where(l => l.RefProduit == query.RefProduit)
            .Select(l => new HistoriqueAchatVenteResponse
            {
                Date = l.NumBonRecNavigation.Date,
                Type = "Achat",
                DocumentNum = l.NumBonRecNavigation.Num,
                FournisseurClient = l.NumBonRecNavigation.IdFournisseurNavigation.Nom,
                Quantite = l.QteLi,
                PrixHt = l.PrixHt,
                TotalHt = l.TotHt
            })
            .ToListAsync(cancellationToken);

        // Récupérer l'historique de vente depuis LigneBl (en ignorant le global query filter)
        var ventes = await _context.LigneBl
            .IgnoreQueryFilters()
            .Include(l => l.NumBlNavigation)
                .ThenInclude(bl => bl.Client)
            .Where(l => l.RefProduit == query.RefProduit)
            .Select(l => new HistoriqueAchatVenteResponse
            {
                Date = l.NumBlNavigation.Date,
                Type = "Vente",
                DocumentNum = l.NumBlNavigation.Num,
                FournisseurClient = l.NumBlNavigation.Client != null ? l.NumBlNavigation.Client.Nom : null,
                Quantite = l.QteLi,
                PrixHt = l.PrixHt,
                TotalHt = l.TotHt
            })
            .ToListAsync(cancellationToken);

        // Combiner et trier par date décroissante
        historique.AddRange(achats);
        historique.AddRange(ventes);
        historique = historique.OrderByDescending(h => h.Date).ToList();

        _logger.LogInformation("Historique récupéré pour {RefProduit}: {Count} entrées", query.RefProduit, historique.Count);
        return Result.Ok(historique);
    }
}

