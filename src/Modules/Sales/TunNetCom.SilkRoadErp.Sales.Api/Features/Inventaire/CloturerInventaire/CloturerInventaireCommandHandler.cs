using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.CloturerInventaire;

public class CloturerInventaireCommandHandler(
    SalesContext _context,
    ILogger<CloturerInventaireCommandHandler> _logger)
    : IRequestHandler<CloturerInventaireCommand, Result>
{
    public async Task<Result> Handle(CloturerInventaireCommand command, CancellationToken cancellationToken)
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
            inventaire.Cloturer();
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Inventaire {Id} closed successfully", command.Id);
            return Result.Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot close inventaire {Id}", command.Id);
            return Result.Fail(ex.Message);
        }
    }
}

