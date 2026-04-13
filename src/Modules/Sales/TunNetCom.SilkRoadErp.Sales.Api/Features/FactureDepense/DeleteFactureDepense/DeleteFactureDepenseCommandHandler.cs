using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.DeleteFactureDepense;

public class DeleteFactureDepenseCommandHandler(
    SalesContext _context,
    ILogger<DeleteFactureDepenseCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<DeleteFactureDepenseCommand, Result>
{
    public async Task<Result> Handle(DeleteFactureDepenseCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteFactureDepenseCommand called with Id {Id}", command.Id);

        var entity = await _context.FactureDepense
            .Include(f => f.PaiementTiersDepenseFactureDepense)
            .FirstOrDefaultAsync(f => f.Id == command.Id, cancellationToken);

        if (entity == null)
            return Result.Fail(EntityNotFound.Error());

        if (entity.PaiementTiersDepenseFactureDepense?.Count > 0)
        {
            _logger.LogWarning("FactureDepense Id {Id} has linked paiements, cannot delete", command.Id);
            return Result.Fail("facture_has_paiement");
        }

        if (!string.IsNullOrWhiteSpace(entity.DocumentStoragePath))
        {
            try
            {
                await _documentStorageService.DeleteAsync(entity.DocumentStoragePath, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error deleting document for FactureDepense Id {Id}, continuing with entity delete", command.Id);
            }
        }

        _context.FactureDepense.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("FactureDepense Id {Id} deleted successfully", command.Id);
        return Result.Ok();
    }
}
