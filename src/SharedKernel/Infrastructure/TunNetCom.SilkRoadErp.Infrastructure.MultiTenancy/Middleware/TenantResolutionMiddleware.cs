using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Resolvers;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Store;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Middleware;

public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var resolvers = context.RequestServices.GetServices<ITenantResolver>()
            .OrderBy(r => r.Priority);

        string? tenantIdentifier = null;
        foreach (var resolver in resolvers)
        {
            tenantIdentifier = await resolver.ResolveAsync(context);
            if (tenantIdentifier is not null)
            {
                _logger.LogDebug("Tenant resolved by {Resolver}: {TenantIdentifier}",
                    resolver.GetType().Name, tenantIdentifier);
                break;
            }
        }

        if (tenantIdentifier is null)
        {
            _logger.LogWarning("No tenant could be resolved for request {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant could not be resolved." });
            return;
        }

        var store = context.RequestServices.GetRequiredService<ITenantStore>();
        var tenantInfo = await store.GetByIdentifierAsync(tenantIdentifier, context.RequestAborted);

        if (tenantInfo is null)
        {
            _logger.LogWarning("Tenant not found: {TenantIdentifier}", tenantIdentifier);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant not found." });
            return;
        }

        if (!tenantInfo.IsActive)
        {
            _logger.LogWarning("Tenant {TenantId} is inactive", tenantInfo.Id);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = "Account suspended." });
            return;
        }

        var tenantContext = context.RequestServices.GetRequiredService<MultiTenantContext>();
        tenantContext.SetTenant(tenantInfo);

        await _next(context);
    }
}
