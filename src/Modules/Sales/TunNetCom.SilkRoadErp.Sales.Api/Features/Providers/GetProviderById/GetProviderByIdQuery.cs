namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProviderById;

public record GetProviderByIdQuery(int Id) : IRequest<Result<ProviderResponse>>;
