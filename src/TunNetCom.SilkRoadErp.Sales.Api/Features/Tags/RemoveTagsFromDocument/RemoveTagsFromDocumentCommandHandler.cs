using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.RemoveTagsFromDocument;

public class RemoveTagsFromDocumentCommandHandler(
    SalesContext _context,
    ILogger<RemoveTagsFromDocumentCommandHandler> _logger)
    : IRequestHandler<RemoveTagsFromDocumentCommand, Result>
{
    public async Task<Result> Handle(RemoveTagsFromDocumentCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing tags from document {DocumentType} with Id {DocumentId}", command.DocumentType, command.DocumentId);

        var documentTags = await _context.DocumentTag
            .Where(dt => dt.DocumentType == command.DocumentType 
                && dt.DocumentId == command.DocumentId 
                && command.TagIds.Contains(dt.TagId))
            .ToListAsync(cancellationToken);

        if (!documentTags.Any())
        {
            return Result.Ok();
        }

        _context.DocumentTag.RemoveRange(documentTags);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Removed {Count} tags from document {DocumentType} with Id {DocumentId}", documentTags.Count, command.DocumentType, command.DocumentId);

        return Result.Ok();
    }
}

