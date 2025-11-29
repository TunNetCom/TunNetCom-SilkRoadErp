using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.DeleteUser;

public class DeleteUserCommandHandler(
    SalesContext _context,
    ILogger<DeleteUserCommandHandler> _logger)
    : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.User
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Fail($"User with ID {request.UserId} not found");
        }

        // Soft delete - deactivate user
        user.UpdateUser(isActive: false);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User deactivated: {Username}, ID: {UserId}", user.Username, user.Id);

        return Result.Ok();
    }
}


