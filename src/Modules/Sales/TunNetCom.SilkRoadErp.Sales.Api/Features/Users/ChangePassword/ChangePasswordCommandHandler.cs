using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.ChangePassword;

public class ChangePasswordCommandHandler(
    SalesContext _context,
    IPasswordHasher _passwordHasher,
    ILogger<ChangePasswordCommandHandler> _logger)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.User
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Fail($"User with ID {request.UserId} not found");
        }

        var newPasswordHash = _passwordHasher.HashPassword(request.Request.NewPassword);
        user.ChangePassword(newPasswordHash);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password changed for user: {Username}, ID: {UserId}", user.Username, user.Id);

        return Result.Ok();
    }
}


