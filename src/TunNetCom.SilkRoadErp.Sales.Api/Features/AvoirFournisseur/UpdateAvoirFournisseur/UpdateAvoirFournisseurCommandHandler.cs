using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.UpdateAvoirFournisseur;

public class UpdateAvoirFournisseurCommandHandler(
    SalesContext _context,
    ILogger<UpdateAvoirFournisseurCommandHandler> _logger)
    : IRequestHandler<UpdateAvoirFournisseurCommand, Result>
{
    public async Task<Result> Handle(UpdateAvoirFournisseurCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateAvoirFournisseurCommand called with Id {Id}", command.Id);

        var avoirFournisseur = await _context.AvoirFournisseur
            .Include(a => a.LigneAvoirFournisseur)
            .FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

        if (avoirFournisseur == null)
        {
            _logger.LogEntityNotFound(nameof(AvoirFournisseur), command.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        if (avoirFournisseur.Statut == DocumentStatus.Valid)
        {
            return Result.Fail("Le document est validé et ne peut plus être modifié.");
        }

        if (command.FournisseurId.HasValue)
        {
            var fournisseurExists = await _context.Fournisseur.AnyAsync(f => f.Id == command.FournisseurId.Value, cancellationToken);
            if (!fournisseurExists)
            {
                return Result.Fail("fournisseur_not_found");
            }
        }

        int? factureAvoirFournisseurId = null;
        if (command.NumFactureAvoirFournisseur.HasValue)
        {
            var factureAvoir = await _context.FactureAvoirFournisseur.FirstOrDefaultAsync(f => f.Id == command.NumFactureAvoirFournisseur.Value, cancellationToken);
            if (factureAvoir == null)
            {
                return Result.Fail("facture_avoir_fournisseur_not_found");
            }
            factureAvoirFournisseurId = factureAvoir.Id;
        }

        // Validate products exist
        var productRefs = command.Lines.Select(l => l.RefProduit).Distinct().ToList();
        var productsExist = await _context.Produit
            .Where(p => productRefs.Contains(p.Refe))
            .Select(p => p.Refe)
            .ToListAsync(cancellationToken);

        var missingProducts = productRefs.Except(productsExist).ToList();
        if (missingProducts.Any())
        {
            _logger.LogWarning("Products not found: {Products}", string.Join(", ", missingProducts));
            return Result.Fail($"products_not_found: {string.Join(", ", missingProducts)}");
        }

        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        // Update avoir fournisseur
        avoirFournisseur.UpdateAvoirFournisseur(
            command.Date,
            command.FournisseurId,
            factureAvoirFournisseurId, // Use Id instead of Num
            activeAccountingYear.Id,
            command.NumAvoirChezFournisseur);

        // Remove existing lines
        _context.LigneAvoirFournisseur.RemoveRange(avoirFournisseur.LigneAvoirFournisseur);

        // Create new lines
        foreach (var lineRequest in command.Lines)
        {
            var remiseAmount = DecimalHelper.RoundAmount(lineRequest.PrixHt * lineRequest.QteLi * (decimal)(lineRequest.Remise / 100.0));
            var totHt = DecimalHelper.RoundAmount((lineRequest.PrixHt * lineRequest.QteLi) - remiseAmount);
            var totTva = DecimalHelper.RoundAmount(totHt * (decimal)(lineRequest.Tva / 100.0));
            var totTtc = DecimalHelper.RoundAmount(totHt + totTva);

            var ligne = new LigneAvoirFournisseur
            {
                AvoirFournisseurId = avoirFournisseur.Id,
                RefProduit = lineRequest.RefProduit,
                DesignationLi = lineRequest.DesignationLi,
                QteLi = lineRequest.QteLi,
                PrixHt = lineRequest.PrixHt,
                Remise = lineRequest.Remise,
                TotHt = totHt,
                Tva = lineRequest.Tva,
                TotTtc = totTtc
            };

            _context.LigneAvoirFournisseur.Add(ligne);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("AvoirFournisseur with Id {Id} updated successfully", avoirFournisseur.Id);
        return Result.Ok();
    }
}

