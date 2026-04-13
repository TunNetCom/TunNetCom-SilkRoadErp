namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.DeletePaiementFournisseur;

public class DeletePaiementFournisseurCommandHandler(
    SalesContext _context,
    ILogger<DeletePaiementFournisseurCommandHandler> _logger)
    : IRequestHandler<DeletePaiementFournisseurCommand, Result>
{
    public async Task<Result> Handle(DeletePaiementFournisseurCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeletePaiementFournisseurCommand called with Id {Id}", command.Id);

        var paiement = await _context.PaiementFournisseur
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (paiement == null)
        {
            return Result.Fail("paiement_fournisseur_not_found");
        }

        _context.PaiementFournisseur.Remove(paiement);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PaiementFournisseur with Id {Id} deleted successfully", command.Id);
        return Result.Ok();
    }
}

