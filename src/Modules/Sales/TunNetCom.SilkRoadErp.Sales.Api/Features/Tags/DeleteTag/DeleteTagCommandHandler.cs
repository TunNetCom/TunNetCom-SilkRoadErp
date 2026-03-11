using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.DeleteTag;

public class DeleteTagCommandHandler(
    SalesContext _context,
    ILogger<DeleteTagCommandHandler> _logger)
    : IRequestHandler<DeleteTagCommand, Result>
{
    public async Task<Result> Handle(DeleteTagCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting tag with Id {TagId}", command.Id);

        var tag = await _context.Tag
            .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

        if (tag == null)
        {
            return Result.Fail("tag_not_found");
        }

        _context.Tag.Remove(tag);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tag {TagId} deleted successfully", command.Id);

        return Result.Ok();
    }
}

