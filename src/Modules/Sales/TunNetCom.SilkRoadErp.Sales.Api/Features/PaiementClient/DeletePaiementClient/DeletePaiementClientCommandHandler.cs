namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.DeletePaiementClient;

public class DeletePaiementClientCommandHandler(
    SalesContext _context,
    ILogger<DeletePaiementClientCommandHandler> _logger)
    : IRequestHandler<DeletePaiementClientCommand, Result>
{
    public async Task<Result> Handle(DeletePaiementClientCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeletePaiementClientCommand called with Id {Id}", command.Id);

        var paiement = await _context.PaiementClient
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (paiement == null)
        {
            return Result.Fail("paiement_client_not_found");
        }

        _context.PaiementClient.Remove(paiement);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PaiementClient with Id {Id} deleted successfully", command.Id);
        return Result.Ok();
    }
}

