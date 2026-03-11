using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.SharedKernel.Caching;

namespace TunNetCom.SilkRoadErp.Infrastructure.Caching;

public static class CachingServiceExtensions
{
    public static IServiceCollection AddTenantCaching(this IServiceCollection services, IConfiguration config)
    {
        var provider = config.GetValue<string>("Caching:Provider") ?? "InMemory";

        if (string.Equals(provider, "Redis", StringComparison.OrdinalIgnoreCase))
        {
            services.AddStackExchangeRedisCache(o => config.GetSection("Caching:Redis").Bind(o));
            services.AddScoped<ITenantCache, RedisTenantCache>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddScoped<ITenantCache, InMemoryTenantCache>();
        }

        return services;
    }
}
