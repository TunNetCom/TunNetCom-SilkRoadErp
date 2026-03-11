using MediatR;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Behaviors;

public sealed class TenantValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ITenantContext _tenantContext;

    public TenantValidationBehavior(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_tenantContext.IsMultiTenant)
            return await next();

        if (!_tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant context is not resolved. Ensure tenant resolution middleware is configured.");

        if (_tenantContext.CurrentTenant is not null && !_tenantContext.CurrentTenant.IsActive)
            throw new InvalidOperationException($"Tenant '{_tenantContext.TenantId}' is not active.");

        return await next();
    }
}
