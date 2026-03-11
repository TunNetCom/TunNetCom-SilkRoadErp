using Microsoft.Extensions.Options;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public sealed class BlazorTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DeploymentOptions _options;

    public BlazorTenantContext(IHttpContextAccessor httpContextAccessor, IOptions<DeploymentOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public string TenantId
    {
        get
        {
            if (!IsMultiTenant)
                return _options.DefaultTenantId;

            var host = _httpContextAccessor.HttpContext?.Request.Host.Host;
            if (host is null)
                return _options.DefaultTenantId;

            if (host.Contains('.'))
            {
                return host.Split('.')[0];
            }

            return _options.DefaultTenantId;
        }
    }

    public TenantInfo? CurrentTenant => null;
    public bool IsResolved => !string.IsNullOrEmpty(TenantId);
    public bool IsMultiTenant => _options.Mode == DeploymentMode.MultiTenant;
}
