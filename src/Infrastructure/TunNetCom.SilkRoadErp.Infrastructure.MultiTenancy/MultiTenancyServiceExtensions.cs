using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Behaviors;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.EfCore;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Features;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Resolvers;
using TunNetCom.SilkRoadErp.SharedKernel.Features;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy;

public static class MultiTenancyServiceExtensions
{
    public static IServiceCollection AddMultiTenancy(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<DeploymentOptions>(config.GetSection(DeploymentOptions.SectionName));

        var mode = config.GetValue<DeploymentMode>("Deployment:Mode");

        if (mode == DeploymentMode.MultiTenant)
        {
            services.AddScoped<MultiTenantContext>();
            services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<MultiTenantContext>());
            services.AddScoped<ITenantResolver, SubdomainTenantResolver>();
            services.AddScoped<ITenantResolver, HeaderTenantResolver>();
            services.AddScoped<ITenantResolver, JwtClaimTenantResolver>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantValidationBehavior<,>));
        }
        else
        {
            services.AddSingleton<ITenantContext, StandaloneTenantContext>();
            services.AddSingleton<IFeatureGate, StandaloneFeatureGate>();
        }

        services.AddScoped<TenantSaveChangesInterceptor>();

        return services;
    }
}
