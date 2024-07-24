namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.DeleteProvider;

public class DeleteProviderCommandHandler(SalesContext _context, ILogger<DeleteProviderCommandHandler> _logger) : IRequestHandler<DeleteProviderCommand, Result>
{
    public async Task<Result> Handle(DeleteProviderCommand deleteProviderCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityDeletionAttempt("Provider", deleteProviderCommand.Id);
        var provider = await _context.Fournisseur.FindAsync(deleteProviderCommand.Id);

        if (provider is null)
        {
            _logger.LogEntityNotFound("Customer", deleteProviderCommand.Id);
            return Result.Fail("provider_not_found");
        }

        _context.Fournisseur.Remove(provider);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityDeleted("Provider", provider.Id);
        return Result.Ok();
    }
}
