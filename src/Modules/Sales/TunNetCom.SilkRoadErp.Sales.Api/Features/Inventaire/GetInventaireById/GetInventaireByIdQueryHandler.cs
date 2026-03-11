using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetInventaireById;

public class GetInventaireByIdQueryHandler(
    SalesContext _context,
    ILogger<GetInventaireByIdQueryHandler> _logger)
    : IRequestHandler<GetInventaireByIdQuery, Result<FullInventaireResponse>>
{
    public async Task<Result<FullInventaireResponse>> Handle(GetInventaireByIdQuery query, CancellationToken cancellationToken)
    {
        var inventaire = await _context.Inventaire
            .Include(i => i.AccountingYear)
            .Include(i => i.LigneInventaire)
                .ThenInclude(l => l.RefProduitNavigation)
            .Where(i => i.Id == query.Id)
            .Select(i => new FullInventaireResponse
            {
                Id = i.Id,
                Num = i.Num,
                AccountingYearId = i.AccountingYearId,
                AccountingYear = i.AccountingYear.Year,
                DateInventaire = i.DateInventaire,
                Description = i.Description,
                Statut = (int)i.Statut,
                StatutLibelle = i.Statut == Domain.Entites.InventaireStatut.Brouillon ? "Brouillon" :
                               i.Statut == Domain.Entites.InventaireStatut.Valide ? "Validé" : "Clôturé",
                Lignes = i.LigneInventaire.Select(l => new LigneInventaireResponse
                {
                    Id = l.Id,
                    InventaireId = l.InventaireId,
                    RefProduit = l.RefProduit,
                    IdProduit = l.RefProduitNavigation.Id,
                    NomProduit = l.RefProduitNavigation.Nom,
                    QuantiteTheorique = l.QuantiteTheorique,
                    QuantiteReelle = l.QuantiteReelle,
                    PrixHt = l.PrixHt,
                    DernierPrixAchat = l.DernierPrixAchat
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (inventaire == null)
        {
            _logger.LogWarning("Inventaire {Id} not found", query.Id);
            return Result.Fail("inventaire_not_found");
        }

        return Result.Ok(inventaire);
    }
}

