using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.CreateTag;

public class CreateTagCommandHandler(
    SalesContext _context,
    ILogger<CreateTagCommandHandler> _logger)
    : IRequestHandler<CreateTagCommand, Result<TagResponse>>
{
    public async Task<Result<TagResponse>> Handle(CreateTagCommand command, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(Domain.Entites.Tag), command);

        var tagExists = await _context.Tag
            .AsNoTracking()
            .AnyAsync(t => t.Name == command.Name, cancellationToken);

        if (tagExists)
        {
            return Result.Fail("tag_name_already_exists");
        }

        var tag = new Domain.Entites.Tag
        {
            Name = command.Name,
            Color = command.Color,
            Description = command.Description
        };

        _context.Tag.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully(nameof(Domain.Entites.Tag), tag.Id);

        return new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name,
            Color = tag.Color,
            Description = tag.Description
        };
    }
}

