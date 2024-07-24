using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProvider;
public record GetProviderQuery (
    int PageNumber,
    int PageSize,
    string? SearchKeyword) : IRequest<PagedList<ProviderResponse>>;
        

    

