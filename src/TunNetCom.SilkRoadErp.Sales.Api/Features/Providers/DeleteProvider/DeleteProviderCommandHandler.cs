using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.DeleteProvider;

public class DeleteProviderCommandHandler(SalesContext _context) : IRequestHandler<DeleteProviderCommand, Result>
{
    public async Task<Result> Handle(DeleteProviderCommand deleteProviderCommand, CancellationToken cancellationToken)
    {

        var provider = await _context.Fournisseur.FindAsync(deleteProviderCommand.Id);

        if (provider is null)
        {
            return Result.Fail("provider_not_found");
        }

        _context.Fournisseur.Remove(provider);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
