using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Auth;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Auth.RefreshToken;

public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, Result<LoginResponse>>
{
    private readonly SalesContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    public RefreshTokenQueryHandler(
        SalesContext context,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponse>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var refreshTokenEntity = await _context.RefreshToken
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(rt => rt.Token == request.Request.RefreshToken, cancellationToken);

        if (refreshTokenEntity == null || !refreshTokenEntity.IsValid)
        {
            return Result.Fail("Invalid refresh token");
        }

        var user = refreshTokenEntity.User;

        if (!user.IsActive)
        {
            return Result.Fail("User account is inactive");
        }

        // Revoke old refresh token
        refreshTokenEntity.Revoke();

        // Get roles and permissions
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        // Generate new tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles, permissions);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpirationDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        var expiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

        // Save new refresh token
        var newRefreshTokenEntity = Domain.Entites.RefreshToken.CreateRefreshToken(user.Id, newRefreshToken, expiresAt);
        _context.RefreshToken.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        var accessTokenExpirationMinutes = int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "15");

        return Result.Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = accessTokenExpirationMinutes * 60, // in seconds
            TokenType = "Bearer"
        });
    }
}

