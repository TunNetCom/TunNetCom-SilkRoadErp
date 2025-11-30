using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.UpdateAvoir;

public class UpdateAvoirCommandHandler(
    SalesContext _context,
    ILogger<UpdateAvoirCommandHandler> _logger)
    : IRequestHandler<UpdateAvoirCommand, Result>
{
    public async Task<Result> Handle(UpdateAvoirCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateAvoirCommand called with Num {Num}", command.Num);

        var avoir = await _context.Avoirs
            .Include(a => a.LigneAvoirs)
            .FirstOrDefaultAsync(a => a.Num == command.Num, cancellationToken);

        if (avoir == null)
        {
            _logger.LogEntityNotFound(nameof(Avoirs), command.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        if (avoir.Statut == DocumentStatus.Valide)
        {
            return Result.Fail("Le document est validé et ne peut plus être modifié.");
        }

        if (command.ClientId.HasValue)
        {
            var clientExists = await _context.Client.AnyAsync(c => c.Id == command.ClientId.Value, cancellationToken);
            if (!clientExists)
            {
                return Result.Fail("client_not_found");
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

        // Update avoir
        avoir.UpdateAvoir(
            command.Date,
            command.ClientId,
            activeAccountingYear.Id);

        // Remove existing lines
        _context.LigneAvoirs.RemoveRange(avoir.LigneAvoirs);

        // Create new lines
        foreach (var lineRequest in command.Lines)
        {
            var remiseAmount = DecimalHelper.RoundAmount(lineRequest.PrixHt * lineRequest.QteLi * (decimal)(lineRequest.Remise / 100.0));
            var totHt = DecimalHelper.RoundAmount((lineRequest.PrixHt * lineRequest.QteLi) - remiseAmount);
            var totTva = DecimalHelper.RoundAmount(totHt * (decimal)(lineRequest.Tva / 100.0));
            var totTtc = DecimalHelper.RoundAmount(totHt + totTva);

            var ligne = new LigneAvoirs
            {
                AvoirsId = avoir.Id,
                RefProduit = lineRequest.RefProduit,
                DesignationLi = lineRequest.DesignationLi,
                QteLi = lineRequest.QteLi,
                PrixHt = lineRequest.PrixHt,
                Remise = lineRequest.Remise,
                TotHt = totHt,
                Tva = lineRequest.Tva,
                TotTtc = totTtc
            };

            _context.LigneAvoirs.Add(ligne);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Avoir with Num {Num} updated successfully", avoir.Num);
        return Result.Ok();
    }
}

