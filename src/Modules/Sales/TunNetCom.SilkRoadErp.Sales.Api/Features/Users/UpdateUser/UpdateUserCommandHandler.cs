using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.UpdateUser;

public class UpdateUserCommandHandler(
    SalesContext _context,
    ILogger<UpdateUserCommandHandler> _logger)
    : IRequestHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.User
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Fail($"User with ID {request.UserId} not found");
        }

        // Update user properties
        user.UpdateUser(
            email: request.Request.Email,
            firstName: request.Request.FirstName,
            lastName: request.Request.LastName,
            isActive: request.Request.IsActive);

        // Update roles if provided
        if (request.Request.RoleIds != null)
        {
            // Remove existing roles
            _context.UserRole.RemoveRange(user.UserRoles);
            
            // Add new roles
            foreach (var roleId in request.Request.RoleIds)
            {
                var userRole = UserRole.CreateUserRole(user.Id, roleId);
                await _context.UserRole.AddAsync(userRole, cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User updated successfully: {Username}, ID: {UserId}", user.Username, user.Id);

        return Result.Ok();
    }
}


