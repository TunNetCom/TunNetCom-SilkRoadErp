using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;

public record class GetAppParametersQuery() : IRequest<Result<GetAppParametersResponse>>;
