using TunNetCom.SilkRoadErp.Sales.Api.Contracts.Providers;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProvider;
    public record GetProviderQuery (
          int PageNumber,
    int PageSize,
    string? SearchKeyword) : IRequest<PagedList<ProviderResponse>>;
        

    

