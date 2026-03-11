using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.GetAllTags;

public record GetAllTagsQuery() : IRequest<List<TagResponse>>;

