using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Resolvers;

public sealed class SubdomainTenantResolver : ITenantResolver
{
    private readonly string? _baseDomain;

    public SubdomainTenantResolver(IConfiguration configuration)
    {
        _baseDomain = configuration["MultiTenancy:TenantResolution:Subdomain:BaseDomain"];
    }

    public int Priority => 1;

    public Task<string?> ResolveAsync(HttpContext context)
    {
        if (string.IsNullOrEmpty(_baseDomain))
            return Task.FromResult<string?>(null);

        var host = context.Request.Host.Host;
        if (host.EndsWith($".{_baseDomain}", StringComparison.OrdinalIgnoreCase))
        {
            var subdomain = host[..^(_baseDomain.Length + 1)];
            if (!string.IsNullOrWhiteSpace(subdomain))
                return Task.FromResult<string?>(subdomain);
        }

        return Task.FromResult<string?>(null);
    }
}
