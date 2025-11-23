using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.CreateAvoirFournisseur;

public class CreateAvoirFournisseurCommandHandler(
    SalesContext _context,
    ILogger<CreateAvoirFournisseurCommandHandler> _logger,
    INumberGeneratorService _numberGeneratorService)
    : IRequestHandler<CreateAvoirFournisseurCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAvoirFournisseurCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateAvoirFournisseurCommand called with FournisseurId {FournisseurId} and Date {Date}", command.FournisseurId, command.Date);

        if (command.FournisseurId.HasValue)
        {
            var fournisseurExists = await _context.Fournisseur.AnyAsync(f => f.Id == command.FournisseurId.Value, cancellationToken);
            if (!fournisseurExists)
            {
                return Result.Fail("fournisseur_not_found");
            }
        }

        if (command.NumFactureAvoirFournisseur.HasValue)
        {
            var factureAvoirExists = await _context.FactureAvoirFournisseur.AnyAsync(f => f.Num == command.NumFactureAvoirFournisseur.Value, cancellationToken);
            if (!factureAvoirExists)
            {
                return Result.Fail("facture_avoir_fournisseur_not_found");
            }
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

        var num = await _numberGeneratorService.GenerateAvoirFournisseurNumberAsync(activeAccountingYear.Id, cancellationToken);

        var avoirFournisseur = Domain.Entites.AvoirFournisseur.CreateAvoirFournisseur(
            command.Date,
            command.FournisseurId,
            command.NumFactureAvoirFournisseur,
            activeAccountingYear.Id);
        avoirFournisseur.Num = num;
        avoirFournisseur.NumAvoirFournisseur = num; // Using the same number for simplicity

        _context.AvoirFournisseur.Add(avoirFournisseur);
        await _context.SaveChangesAsync(cancellationToken); // Save to get the Id

        // Create lines and calculate totals
        foreach (var lineRequest in command.Lines)
        {
            var remiseAmount = lineRequest.PrixHt * lineRequest.QteLi * (decimal)(lineRequest.Remise / 100.0);
            var totHt = (lineRequest.PrixHt * lineRequest.QteLi) - remiseAmount;
            var totTva = totHt * (decimal)(lineRequest.Tva / 100.0);
            var totTtc = totHt + totTva;

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
        _logger.LogInformation("AvoirFournisseur created successfully with Num {Num}", avoirFournisseur.Num);
        return Result.Ok(avoirFournisseur.Num);
    }
}

