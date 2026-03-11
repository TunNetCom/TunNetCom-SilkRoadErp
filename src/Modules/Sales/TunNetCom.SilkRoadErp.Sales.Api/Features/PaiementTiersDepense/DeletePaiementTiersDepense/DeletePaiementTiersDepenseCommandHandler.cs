using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.DeletePaiementTiersDepense;

public class DeletePaiementTiersDepenseCommandHandler(
    SalesContext _context,
    ILogger<DeletePaiementTiersDepenseCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<DeletePaiementTiersDepenseCommand, Result>
{
    public async Task<Result> Handle(DeletePaiementTiersDepenseCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeletePaiementTiersDepenseCommand called with Id {Id}", command.Id);

        var entity = await _context.PaiementTiersDepense
            .Include(p => p.FactureDepenses)
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (entity == null)
            return Result.Fail(EntityNotFound.Error());

        if (!string.IsNullOrWhiteSpace(entity.DocumentStoragePath))
        {
            try
            {
                await _documentStorageService.DeleteAsync(entity.DocumentStoragePath, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error deleting document for PaiementTiersDepense Id {Id}, continuing with entity delete", command.Id);
            }
        }

        _context.PaiementTiersDepense.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PaiementTiersDepense Id {Id} deleted successfully", command.Id);
        return Result.Ok();
    }
}
