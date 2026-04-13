using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.CreateTag;

public record CreateTagCommand(
    string Name,
    string? Color,
    string? Description
) : IRequest<Result<TagResponse>>;

