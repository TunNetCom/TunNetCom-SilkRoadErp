using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.CreateUser;

public class CreateUserCommandHandler(
    SalesContext _context,
    IPasswordHasher _passwordHasher,
    ILogger<CreateUserCommandHandler> _logger)
    : IRequestHandler<CreateUserCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if username already exists
        var existingUser = await _context.User
            .FirstOrDefaultAsync(u => u.Username == request.Request.Username, cancellationToken);

        if (existingUser != null)
        {
            return Result.Fail("Username already exists");
        }

        // Check if email already exists
        var existingEmail = await _context.User
            .FirstOrDefaultAsync(u => u.Email == request.Request.Email, cancellationToken);

        if (existingEmail != null)
        {
            return Result.Fail("Email already exists");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Request.Password);

        // Create user
        var user = User.CreateUser(
            username: request.Request.Username,
            email: request.Request.Email,
            passwordHash: passwordHash,
            firstName: request.Request.FirstName,
            lastName: request.Request.LastName,
            isActive: request.Request.IsActive);

        await _context.User.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Assign roles if provided
        if (request.Request.RoleIds.Any())
        {
            foreach (var roleId in request.Request.RoleIds)
            {
                var userRole = UserRole.CreateUserRole(user.Id, roleId);
                await _context.UserRole.AddAsync(userRole, cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("User created successfully: {Username}, ID: {UserId}", user.Username, user.Id);

        return Result.Ok(user.Id);
    }
}


