using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.UpdateTag;

public class UpdateTagCommandHandler(
    SalesContext _context,
    ILogger<UpdateTagCommandHandler> _logger)
    : IRequestHandler<UpdateTagCommand, Result<TagResponse>>
{
    public async Task<Result<TagResponse>> Handle(UpdateTagCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating tag with Id {TagId}", command.Id);

        var tag = await _context.Tag
            .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

        if (tag == null)
        {
            return Result.Fail("tag_not_found");
        }

        var tagNameExists = await _context.Tag
            .AsNoTracking()
            .AnyAsync(t => t.Name == command.Name && t.Id != command.Id, cancellationToken);

        if (tagNameExists)
        {
            return Result.Fail("tag_name_already_exists");
        }

        tag.Name = command.Name;
        tag.Color = command.Color;
        tag.Description = command.Description;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tag {TagId} updated successfully", command.Id);

        return new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name,
            Color = tag.Color,
            Description = tag.Description
        };
    }
}

