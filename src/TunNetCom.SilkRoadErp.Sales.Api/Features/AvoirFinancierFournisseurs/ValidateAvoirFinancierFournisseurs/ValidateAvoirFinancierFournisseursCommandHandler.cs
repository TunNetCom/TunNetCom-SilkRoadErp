namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.ValidateAvoirFinancierFournisseurs;

public class ValidateAvoirFinancierFournisseursCommandHandler(
    SalesContext _context,
    ILogger<ValidateAvoirFinancierFournisseursCommandHandler> _logger)
    : IRequestHandler<ValidateAvoirFinancierFournisseursCommand, Result>
{
    public async Task<Result> Handle(ValidateAvoirFinancierFournisseursCommand command, CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var avoirFinanciers = await _context.AvoirFinancierFournisseurs
            .Where(a => command.Ids.Contains(a.Num))
            .ToListAsync(cancellationToken);

        if (avoirFinanciers.Count != command.Ids.Count)
        {
            var foundIds = avoirFinanciers.Select(a => a.Num).ToList();
            var missingIds = command.Ids.Except(foundIds).ToList();
            _logger.LogWarning("AvoirFinancierFournisseurs not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"avoir_financier_fournisseurs_not_found: {string.Join(", ", missingIds)}");
        }

        // AvoirFinancierFournisseurs doesn't have a status field, so we just verify they exist
        _logger.LogInformation("Validated {Count} avoir financier fournisseurs", avoirFinanciers.Count);

        return Result.Ok();
    }
}

