using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.GetAllTags;

public class GetAllTagsQueryHandler(
    SalesContext _context,
    ILogger<GetAllTagsQueryHandler> _logger)
    : IRequestHandler<GetAllTagsQuery, List<TagResponse>>
{
    public async Task<List<TagResponse>> Handle(GetAllTagsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all tags");

        var tags = await _context.Tag
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TagResponse
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
                Description = t.Description
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} tags", tags.Count);

        return tags;
    }
}

