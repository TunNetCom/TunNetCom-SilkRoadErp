using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.AddTagsToDocument;

public class AddTagsToDocumentCommandHandler(
    SalesContext _context,
    ILogger<AddTagsToDocumentCommandHandler> _logger)
    : IRequestHandler<AddTagsToDocumentCommand, Result>
{
    public async Task<Result> Handle(AddTagsToDocumentCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding tags to document {DocumentType} with Id {DocumentId}", command.DocumentType, command.DocumentId);

        // Vérifier que tous les tags existent
        var existingTags = await _context.Tag
            .Where(t => command.TagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        var missingTags = command.TagIds.Except(existingTags).ToList();
        if (missingTags.Any())
        {
            return Result.Fail($"tags_not_found: {string.Join(", ", missingTags)}");
        }

        // Récupérer les tags déjà associés au document
        var existingDocumentTags = await _context.DocumentTag
            .Where(dt => dt.DocumentType == command.DocumentType && dt.DocumentId == command.DocumentId)
            .Select(dt => dt.TagId)
            .ToListAsync(cancellationToken);

        // Filtrer les tags qui ne sont pas déjà associés
        var tagsToAdd = command.TagIds.Except(existingDocumentTags).ToList();

        if (!tagsToAdd.Any())
        {
            return Result.Ok();
        }

        // Ajouter les nouveaux tags
        var documentTags = tagsToAdd.Select(tagId => new Domain.Entites.DocumentTag
        {
            DocumentType = command.DocumentType,
            DocumentId = command.DocumentId,
            TagId = tagId
        }).ToList();

        _context.DocumentTag.AddRange(documentTags);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Added {Count} tags to document {DocumentType} with Id {DocumentId}", tagsToAdd.Count, command.DocumentType, command.DocumentId);

        return Result.Ok();
    }
}

