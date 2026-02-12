namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.DetachAvoirFinancierFromInvoice;

public class DetachAvoirFinancierFromInvoiceCommandHandler(
    SalesContext _context,
    ILogger<DetachAvoirFinancierFromInvoiceCommandHandler> _logger)
    : IRequestHandler<DetachAvoirFinancierFromInvoiceCommand, Result>
{
    public async Task<Result> Handle(DetachAvoirFinancierFromInvoiceCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DetachAvoirFinancierFromInvoiceCommand called for AvoirFinancier Id {Id}", command.Id);

        var avoirFinancier = await _context.AvoirFinancierFournisseurs
            .FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

        if (avoirFinancier == null)
        {
            _logger.LogWarning("AvoirFinancierFournisseurs with Id {Id} not found", command.Id);
            return Result.Fail("avoir_financier_not_found");
        }

        avoirFinancier.NumFactureFournisseur = null;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AvoirFinancierFournisseurs Id {Id} detached from invoice", command.Id);
        return Result.Ok();
    }
}
