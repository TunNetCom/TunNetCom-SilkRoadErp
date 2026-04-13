using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Resolvers;

public sealed class JwtClaimTenantResolver : ITenantResolver
{
    private readonly string _claimType;

    public JwtClaimTenantResolver(IConfiguration configuration)
    {
        _claimType = configuration["MultiTenancy:TenantResolution:JwtClaim:ClaimType"] ?? "tenant_id";
    }

    public int Priority => 3;

    public Task<string?> ResolveAsync(HttpContext context)
    {
        var claim = context.User?.FindFirst(_claimType);
        if (claim is not null && !string.IsNullOrWhiteSpace(claim.Value))
            return Task.FromResult<string?>(claim.Value);

        return Task.FromResult<string?>(null);
    }
}
