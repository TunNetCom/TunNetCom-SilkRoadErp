using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Auth;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Auth.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<LoginResponse>>
{
    private readonly SalesContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginQueryHandler> _logger;

    public LoginQueryHandler(
        SalesContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration,
        ILogger<LoginQueryHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.User
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Username == request.Request.Username && u.IsActive, cancellationToken);

        if (user == null)
        {
            return Result.Fail("Invalid username or password");
        }

        if (!_passwordHasher.VerifyPassword(request.Request.Password, user.PasswordHash))
        {
            return Result.Fail("Invalid username or password");
        }

        // Get roles and permissions
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        // LOG: Debug permissions loading
        _logger.LogInformation("=== LOGIN DEBUG ===");
        _logger.LogInformation("User: {Username} (ID: {UserId})", user.Username, user.Id);
        _logger.LogInformation("UserRoles count: {Count}", user.UserRoles.Count);
        foreach (var userRole in user.UserRoles)
        {
            _logger.LogInformation("  - Role: {RoleName} (ID: {RoleId}), RolePermissions count: {Count}", 
                userRole.Role.Name, userRole.Role.Id, userRole.Role.RolePermissions.Count);
        }
        _logger.LogInformation("Total Roles: {Count} - {Roles}", roles.Count, string.Join(", ", roles));
        _logger.LogInformation("Total Permissions: {Count}", permissions.Count);
        if (permissions.Count > 0)
        {
            _logger.LogInformation("First 10 permissions: {Permissions}", string.Join(", ", permissions.Take(10)));
        }
        else
        {
            _logger.LogWarning("⚠️⚠️⚠️ NO PERMISSIONS LOADED FOR USER {Username}! ⚠️⚠️⚠️", user.Username);
        }

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpirationDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        var expiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

        // Save refresh token
        var refreshTokenEntity = Domain.Entites.RefreshToken.CreateRefreshToken(user.Id, refreshToken, expiresAt);
        _context.RefreshToken.Add(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        const int accessTokenExpirationMinutes = 240; // Hard-coded to 4 hours

        return Result.Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = accessTokenExpirationMinutes * 60, // in seconds (14400)
            TokenType = "Bearer"
        });
    }
}

