using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.GetDocumentTags;

public class GetDocumentTagsQueryHandler(
    SalesContext _context,
    ILogger<GetDocumentTagsQueryHandler> _logger)
    : IRequestHandler<GetDocumentTagsQuery, DocumentTagResponse>
{
    public async Task<DocumentTagResponse> Handle(GetDocumentTagsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting tags for document {DocumentType} with Id {DocumentId}", query.DocumentType, query.DocumentId);

        var tags = await _context.DocumentTag
            .AsNoTracking()
            .Where(dt => dt.DocumentType == query.DocumentType && dt.DocumentId == query.DocumentId)
            .Include(dt => dt.Tag)
            .Select(dt => new TagResponse
            {
                Id = dt.Tag.Id,
                Name = dt.Tag.Name,
                Color = dt.Tag.Color,
                Description = dt.Tag.Description
            })
            .ToListAsync(cancellationToken);

        return new DocumentTagResponse
        {
            DocumentType = query.DocumentType,
            DocumentId = query.DocumentId,
            Tags = tags
        };
    }
}

