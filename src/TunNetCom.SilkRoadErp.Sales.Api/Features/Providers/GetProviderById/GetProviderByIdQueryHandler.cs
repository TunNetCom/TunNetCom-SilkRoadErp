using TunNetCom.SilkRoadErp.Sales.Api.Contracts.Providers;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProviderById;

public class GetProviderByIdQueryHandler(SalesContext _context)
    : IRequestHandler<GetProviderByIdQuery, Result<ProviderResponse>>
{
    public async Task<Result<ProviderResponse>> Handle(GetProviderByIdQuery request, CancellationToken cancellationToken)
    {

        var provider = await _context.Fournisseur.FindAsync(request.Id);
        if (provider is null)
        {
            return Result.Fail("provider_not_found");
        }

        return provider.Adapt<ProviderResponse>();
    }
}