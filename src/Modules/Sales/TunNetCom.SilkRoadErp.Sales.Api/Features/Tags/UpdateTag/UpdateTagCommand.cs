using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.UpdateTag;

public record UpdateTagCommand(
    int Id,
    string Name,
    string? Color,
    string? Description
) : IRequest<Result<TagResponse>>;

