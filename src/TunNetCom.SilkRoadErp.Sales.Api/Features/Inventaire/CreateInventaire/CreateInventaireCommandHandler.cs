using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.CreateInventaire;

public class CreateInventaireCommandHandler(
    SalesContext _context,
    ILogger<CreateInventaireCommandHandler> _logger)
    : IRequestHandler<CreateInventaireCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateInventaireCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating inventaire for AccountingYearId: {AccountingYearId}", command.AccountingYearId);

        // Vérifier que l'exercice comptable existe
        var accountingYear = await _context.AccountingYear
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(ay => ay.Id == command.AccountingYearId, cancellationToken);

        if (accountingYear == null)
        {
            _logger.LogWarning("Accounting year {AccountingYearId} not found", command.AccountingYearId);
            return Result.Fail("accounting_year_not_found");
        }

        // Générer le numéro d'inventaire
        var dernierNum = await _context.Inventaire
            .Where(i => i.AccountingYearId == command.AccountingYearId)
            .OrderByDescending(i => i.Num)
            .Select(i => (int?)i.Num)
            .FirstOrDefaultAsync(cancellationToken);
        
        var nouveauNum = (dernierNum ?? 0) + 1;

        // Créer l'inventaire
        var inventaire = Domain.Entites.Inventaire.CreateInventaire(
            num: nouveauNum,
            accountingYearId: command.AccountingYearId,
            dateInventaire: command.DateInventaire,
            description: command.Description
        );

        await _context.Inventaire.AddAsync(inventaire, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Créer les lignes d'inventaire
        var lignes = new List<LigneInventaire>();
        foreach (var ligneCmd in command.Lignes)
        {
            // Récupérer le produit pour obtenir la quantité théorique
            var produit = await _context.Produit
                .FirstOrDefaultAsync(p => p.Refe == ligneCmd.RefProduit, cancellationToken);

            if (produit == null)
            {
                _logger.LogWarning("Product {RefProduit} not found", ligneCmd.RefProduit);
                continue;
            }

            var ligne = LigneInventaire.CreateLigneInventaire(
                inventaireId: inventaire.Id,
                refProduit: ligneCmd.RefProduit,
                quantiteTheorique: produit.Qte,
                quantiteReelle: ligneCmd.QuantiteReelle,
                prixHt: ligneCmd.PrixHt,
                dernierPrixAchat: ligneCmd.DernierPrixAchat
            );

            lignes.Add(ligne);
        }

        await _context.LigneInventaire.AddRangeAsync(lignes, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Inventaire {Id} created successfully with {Count} lignes", inventaire.Id, lignes.Count);
        return Result.Ok(inventaire.Id);
    }
}

