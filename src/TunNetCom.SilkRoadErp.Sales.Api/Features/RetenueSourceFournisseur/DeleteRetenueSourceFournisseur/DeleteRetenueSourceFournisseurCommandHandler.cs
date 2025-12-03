using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.DeleteRetenueSourceFournisseur;

public class DeleteRetenueSourceFournisseurCommandHandler(
    SalesContext _context,
    ILogger<DeleteRetenueSourceFournisseurCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<DeleteRetenueSourceFournisseurCommand, Result>
{
    public async Task<Result> Handle(DeleteRetenueSourceFournisseurCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteRetenueSourceFournisseurCommand called for FactureFournisseur {NumFactureFournisseur}", command.NumFactureFournisseur);

        var retenue = await _context.RetenueSourceFournisseur
            .FirstOrDefaultAsync(r => r.NumFactureFournisseur == command.NumFactureFournisseur, cancellationToken);

        if (retenue == null)
        {
            _logger.LogEntityNotFound(nameof(RetenueSourceFournisseur), command.NumFactureFournisseur);
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

        _context.RetenueSourceFournisseur.Remove(retenue);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RetenueSourceFournisseur deleted successfully for FactureFournisseur {NumFactureFournisseur}", command.NumFactureFournisseur);
        return Result.Ok();
    }
}

