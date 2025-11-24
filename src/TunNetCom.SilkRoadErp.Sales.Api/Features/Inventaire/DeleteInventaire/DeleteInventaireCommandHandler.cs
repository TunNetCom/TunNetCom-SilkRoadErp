using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.DeleteInventaire;

public class DeleteInventaireCommandHandler(
    SalesContext _context,
    ILogger<DeleteInventaireCommandHandler> _logger)
    : IRequestHandler<DeleteInventaireCommand, Result>
{
    public async Task<Result> Handle(DeleteInventaireCommand command, CancellationToken cancellationToken)
    {
        var inventaire = await _context.Inventaire
            .FirstOrDefaultAsync(i => i.Id == command.Id, cancellationToken);

        if (inventaire == null)
        {
            _logger.LogWarning("Inventaire {Id} not found", command.Id);
            return Result.Fail("inventaire_not_found");
        }

        // Vérifier que l'inventaire est en brouillon
        if (inventaire.Statut != Domain.Entites.InventaireStatut.Brouillon)
        {
            _logger.LogWarning("Cannot delete inventaire {Id} with statut {Statut}", command.Id, inventaire.Statut);
            return Result.Fail("can_only_delete_brouillon_inventaire");
        }

        _context.Inventaire.Remove(inventaire);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Inventaire {Id} deleted successfully", command.Id);
        return Result.Ok();
    }
}

