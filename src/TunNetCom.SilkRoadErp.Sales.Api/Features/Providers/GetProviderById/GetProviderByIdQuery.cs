using TunNetCom.SilkRoadErp.Sales.Api.Contracts.Providers;
namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProviderById;
public class GetProviderByIdQuery : IRequest<Result<ProviderResponse>>
{
    public int Id { get; set; }

    public GetProviderByIdQuery(int id)
    {
        Id = id;
    }
}

