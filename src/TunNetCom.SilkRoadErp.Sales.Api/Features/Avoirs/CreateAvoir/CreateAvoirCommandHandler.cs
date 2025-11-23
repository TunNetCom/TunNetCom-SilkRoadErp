using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.CreateAvoir;

public class CreateAvoirCommandHandler(
    SalesContext _context,
    ILogger<CreateAvoirCommandHandler> _logger,
    INumberGeneratorService _numberGeneratorService)
    : IRequestHandler<CreateAvoirCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAvoirCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateAvoirCommand called with ClientId {ClientId} and Date {Date}", command.ClientId, command.Date);

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

        var num = await _numberGeneratorService.GenerateAvoirNumberAsync(activeAccountingYear.Id, cancellationToken);

        var avoir = Domain.Entites.Avoirs.CreateAvoir(
            command.Date,
            command.ClientId,
            activeAccountingYear.Id);
        avoir.Num = num;

        _context.Avoirs.Add(avoir);
        await _context.SaveChangesAsync(cancellationToken); // Save to get the Id

        // Create lines and calculate totals
        foreach (var lineRequest in command.Lines)
        {
            var remiseAmount = lineRequest.PrixHt * lineRequest.QteLi * (decimal)(lineRequest.Remise / 100.0);
            var totHt = (lineRequest.PrixHt * lineRequest.QteLi) - remiseAmount;
            var totTva = totHt * (decimal)(lineRequest.Tva / 100.0);
            var totTtc = totHt + totTva;

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
        _logger.LogInformation("Avoir created successfully with Num {Num}", avoir.Num);
        return Result.Ok(avoir.Num);
    }
}

