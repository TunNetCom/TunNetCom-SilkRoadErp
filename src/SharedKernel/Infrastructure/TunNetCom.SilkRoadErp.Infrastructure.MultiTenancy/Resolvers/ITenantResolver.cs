using Microsoft.AspNetCore.Http;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Resolvers;

public interface ITenantResolver
{
    int Priority { get; }
    Task<string?> ResolveAsync(HttpContext context);
}
