using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.EfCore;

public static class TenantQueryFilterExtensions
{
    public static void ApplyTenantQueryFilters(this ModelBuilder modelBuilder, ITenantContext tenantContext)
    {
        if (!tenantContext.IsMultiTenant)
            return;

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var tenantProperty = Expression.Property(parameter, nameof(ITenantEntity.TenantId));

            var tenantContextExpr = Expression.Constant(tenantContext);
            var currentTenantId = Expression.Property(tenantContextExpr, nameof(ITenantContext.TenantId));

            var filter = Expression.Lambda(
                Expression.Equal(tenantProperty, currentTenantId),
                parameter);

            entityType.SetQueryFilter(filter);
        }
    }
}
