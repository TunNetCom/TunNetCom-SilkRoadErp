using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.ValiderInventaire;

public class ValiderInventaireCommandHandler(
    SalesContext _context,
    ILogger<ValiderInventaireCommandHandler> _logger)
    : IRequestHandler<ValiderInventaireCommand, Result>
{
    public async Task<Result> Handle(ValiderInventaireCommand command, CancellationToken cancellationToken)
    {
        var inventaire = await _context.Inventaire
            .FirstOrDefaultAsync(i => i.Id == command.Id, cancellationToken);

        if (inventaire == null)
        {
            _logger.LogWarning("Inventaire {Id} not found", command.Id);
            return Result.Fail("inventaire_not_found");
        }

        try
        {
            inventaire.Valider();
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Inventaire {Id} validated successfully", command.Id);
            return Result.Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot validate inventaire {Id}", command.Id);
            return Result.Fail(ex.Message);
        }
    }
}

