namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.DeleteAvoirFinancierFournisseurs;

public class DeleteAvoirFinancierFournisseursCommandHandler(
    SalesContext _context,
    ILogger<DeleteAvoirFinancierFournisseursCommandHandler> _logger)
    : IRequestHandler<DeleteAvoirFinancierFournisseursCommand, Result>
{
    public async Task<Result> Handle(DeleteAvoirFinancierFournisseursCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteAvoirFinancierFournisseursCommand called with Id {Id}", command.Id);

        var avoirFinancier = await _context.AvoirFinancierFournisseurs
            .FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

        if (avoirFinancier == null)
        {
            _logger.LogWarning("AvoirFinancierFournisseurs with Id {Id} not found", command.Id);
            return Result.Fail("avoir_financier_not_found");
        }

        _context.AvoirFinancierFournisseurs.Remove(avoirFinancier);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AvoirFinancierFournisseurs Id {Id} deleted successfully", command.Id);
        return Result.Ok();
    }
}
