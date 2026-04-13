namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.AttachAvoirFinancierToInvoice;

public class AttachAvoirFinancierToInvoiceCommandHandler(
    SalesContext _context,
    ILogger<AttachAvoirFinancierToInvoiceCommandHandler> _logger)
    : IRequestHandler<AttachAvoirFinancierToInvoiceCommand, Result>
{
    public async Task<Result> Handle(AttachAvoirFinancierToInvoiceCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AttachAvoirFinancierToInvoiceCommand called for AvoirFinancier Id {Id} to invoice Num {NumFactureFournisseur}", command.Id, command.NumFactureFournisseur);

        var avoirFinancier = await _context.AvoirFinancierFournisseurs
            .FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

        if (avoirFinancier == null)
        {
            _logger.LogWarning("AvoirFinancierFournisseurs with Id {Id} not found", command.Id);
            return Result.Fail("avoir_financier_not_found");
        }

        var factureExists = await _context.FactureFournisseur
            .AnyAsync(f => f.Num == command.NumFactureFournisseur, cancellationToken);

        if (!factureExists)
        {
            _logger.LogWarning("FactureFournisseur with Num {Num} not found", command.NumFactureFournisseur);
            return Result.Fail("facture_fournisseur_not_found");
        }

        avoirFinancier.NumFactureFournisseur = command.NumFactureFournisseur;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AvoirFinancierFournisseurs Id {Id} attached to invoice Num {NumFactureFournisseur}", command.Id, command.NumFactureFournisseur);
        return Result.Ok();
    }
}
