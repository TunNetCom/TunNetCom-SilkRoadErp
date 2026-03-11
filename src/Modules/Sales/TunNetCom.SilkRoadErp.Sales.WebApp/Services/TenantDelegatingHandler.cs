using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public sealed class TenantDelegatingHandler : DelegatingHandler
{
    private readonly ITenantContext _tenantContext;

    public TenantDelegatingHandler(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_tenantContext.IsMultiTenant && _tenantContext.IsResolved)
        {
            request.Headers.TryAddWithoutValidation("X-Tenant-Id", _tenantContext.TenantId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
