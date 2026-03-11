using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Resolvers;

public sealed class HeaderTenantResolver : ITenantResolver
{
    private readonly string _headerName;

    public HeaderTenantResolver(IConfiguration configuration)
    {
        _headerName = configuration["MultiTenancy:TenantResolution:Header:HeaderName"] ?? "X-Tenant-Id";
    }

    public int Priority => 2;

    public Task<string?> ResolveAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(_headerName, out var values))
        {
            var value = values.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(value))
                return Task.FromResult<string?>(value);
        }

        return Task.FromResult<string?>(null);
    }
}
