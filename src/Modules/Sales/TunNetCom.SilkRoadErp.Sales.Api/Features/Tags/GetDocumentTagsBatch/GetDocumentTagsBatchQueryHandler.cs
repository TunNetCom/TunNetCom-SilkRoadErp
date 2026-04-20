using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.GetDocumentTagsBatch;

public class GetDocumentTagsBatchQueryHandler(
    SalesContext _context,
    ILogger<GetDocumentTagsBatchQueryHandler> _logger)
    : IRequestHandler<GetDocumentTagsBatchQuery, GetDocumentTagsBatchResponse>
{
    public async Task<GetDocumentTagsBatchResponse> Handle(GetDocumentTagsBatchQuery query, CancellationToken cancellationToken)
    {
        var documentIds = query.DocumentIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (string.IsNullOrWhiteSpace(query.DocumentType) || documentIds.Count == 0)
        {
            return new GetDocumentTagsBatchResponse
            {
                DocumentType = query.DocumentType ?? string.Empty,
                Documents = new List<DocumentTagResponse>()
            };
        }

        _logger.LogInformation(
            "Getting tags batch for DocumentType={DocumentType} count={Count}",
            query.DocumentType, documentIds.Count);

        var rows = await _context.DocumentTag
            .AsNoTracking()
            .Where(dt => dt.DocumentType == query.DocumentType && documentIds.Contains(dt.DocumentId))
            .Include(dt => dt.Tag)
            .Select(dt => new
            {
                dt.DocumentId,
                Tag = new TagResponse
                {
                    Id = dt.Tag.Id,
                    Name = dt.Tag.Name,
                    Color = dt.Tag.Color,
                    Description = dt.Tag.Description
                }
            })
            .ToListAsync(cancellationToken);

        var documents = rows
            .GroupBy(r => r.DocumentId)
            .Select(g => new DocumentTagResponse
            {
                DocumentType = query.DocumentType,
                DocumentId = g.Key,
                Tags = g.Select(x => x.Tag).ToList()
            })
            .ToList();

        // Ensure response includes empty lists for ids with no tags (helps the UI avoid per-row fallbacks).
        var byId = documents.ToDictionary(d => d.DocumentId);
        foreach (var id in documentIds)
        {
            if (!byId.ContainsKey(id))
            {
                documents.Add(new DocumentTagResponse
                {
                    DocumentType = query.DocumentType,
                    DocumentId = id,
                    Tags = new List<TagResponse>()
                });
            }
        }

        return new GetDocumentTagsBatchResponse
        {
            DocumentType = query.DocumentType,
            Documents = documents
        };
    }
}

