using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.DeleteRetenueSourceClient;

public class DeleteRetenueSourceClientCommandHandler(
    SalesContext _context,
    ILogger<DeleteRetenueSourceClientCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<DeleteRetenueSourceClientCommand, Result>
{
    public async Task<Result> Handle(DeleteRetenueSourceClientCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteRetenueSourceClientCommand called for Facture {NumFacture}", command.NumFacture);

        var retenue = await _context.RetenueSourceClient
            .FirstOrDefaultAsync(r => r.NumFacture == command.NumFacture, cancellationToken);

        if (retenue == null)
        {
            _logger.LogEntityNotFound(nameof(RetenueSourceClient), command.NumFacture);
            return Result.Fail(EntityNotFound.Error());
        }

        // Delete PDF if exists
        if (!string.IsNullOrWhiteSpace(retenue.PdfStoragePath))
        {
            try
            {
                await _documentStorageService.DeleteAsync(retenue.PdfStoragePath, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error deleting PDF, continuing with record deletion");
            }
        }

        _context.RetenueSourceClient.Remove(retenue);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RetenueSourceClient deleted successfully for Facture {NumFacture}", command.NumFacture);
        return Result.Ok();
    }
}

