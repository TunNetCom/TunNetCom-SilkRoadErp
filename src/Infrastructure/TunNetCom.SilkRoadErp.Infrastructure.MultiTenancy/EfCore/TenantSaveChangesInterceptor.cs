using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.EfCore;

public sealed class TenantSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<TenantSaveChangesInterceptor> _logger;

    public TenantSaveChangesInterceptor(ITenantContext tenantContext, ILogger<TenantSaveChangesInterceptor> logger)
    {
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetTenantId(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetTenantId(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetTenantId(DbContext? context)
    {
        if (context is null)
            return;

        var tenantId = _tenantContext.TenantId;

        foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.TenantId = tenantId;
                    break;

                case EntityState.Modified:
                    if (_tenantContext.IsMultiTenant && entry.Entity.TenantId != tenantId)
                    {
                        _logger.LogError(
                            "Cross-tenant write attempt: entity {Entity} has TenantId={EntityTenant} but current tenant is {CurrentTenant}",
                            entry.Entity.GetType().Name, entry.Entity.TenantId, tenantId);
                        throw new InvalidOperationException("Cross-tenant data modification is not allowed.");
                    }
                    break;
            }
        }
    }
}
