using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.AddTagsToDocumentByName;

public class AddTagsToDocumentByNameCommandHandler(
    SalesContext _context,
    ILogger<AddTagsToDocumentByNameCommandHandler> _logger)
    : IRequestHandler<AddTagsToDocumentByNameCommand, Result>
{
    public async Task<Result> Handle(AddTagsToDocumentByNameCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding tags by name to document {DocumentType} with Id {DocumentId}", command.DocumentType, command.DocumentId);

        var documentTags = new List<Domain.Entites.DocumentTag>();

        foreach (var tagName in command.TagNames.Where(tn => !string.IsNullOrWhiteSpace(tn)))
        {
            var normalizedTagName = tagName.Trim();

            // Chercher ou créer le tag
            var tag = await _context.Tag
                .FirstOrDefaultAsync(t => t.Name == normalizedTagName, cancellationToken);

            if (tag == null)
            {
                // Créer le tag automatiquement
                tag = new Domain.Entites.Tag
                {
                    Name = normalizedTagName
                };
                _context.Tag.Add(tag);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Created new tag: {TagName}", normalizedTagName);
            }

            // Vérifier si le tag n'est pas déjà associé au document
            var existingDocumentTag = await _context.DocumentTag
                .AnyAsync(dt => dt.DocumentType == command.DocumentType 
                    && dt.DocumentId == command.DocumentId 
                    && dt.TagId == tag.Id, cancellationToken);

            if (!existingDocumentTag)
            {
                documentTags.Add(new Domain.Entites.DocumentTag
                {
                    DocumentType = command.DocumentType,
                    DocumentId = command.DocumentId,
                    TagId = tag.Id
                });
            }
        }

        if (documentTags.Any())
        {
            _context.DocumentTag.AddRange(documentTags);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Added {Count} tags to document {DocumentType} with Id {DocumentId}", documentTags.Count, command.DocumentType, command.DocumentId);
        }

        return Result.Ok();
    }
}

