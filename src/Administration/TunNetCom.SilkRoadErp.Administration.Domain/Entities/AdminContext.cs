using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Administration.Domain.Enums;

namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class AdminContext : DbContext
{
    public AdminContext(DbContextOptions<AdminContext> options) : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<BoundedContext> BoundedContexts => Set<BoundedContext>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanBoundedContext> PlanBoundedContexts => Set<PlanBoundedContext>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<TenantBoundedContext> TenantBoundedContexts => Set<TenantBoundedContext>();
    public DbSet<TenantFeatureOverride> TenantFeatureOverrides => Set<TenantFeatureOverride>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<CustomerAccount> CustomerAccounts => Set<CustomerAccount>();
    public DbSet<BillingInvoice> BillingInvoices => Set<BillingInvoice>();
    public DbSet<FeaturePermission> FeaturePermissions => Set<FeaturePermission>();
    public DbSet<FeatureEndpointTag> FeatureEndpointTags => Set<FeatureEndpointTag>();
    public DbSet<FeatureRoute> FeatureRoutes => Set<FeatureRoute>();
    public DbSet<TenantTheme> TenantThemes => Set<TenantTheme>();
    public DbSet<AdminAuditLog> AdminAuditLogs => Set<AdminAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasIndex(t => t.Identifier).IsUnique();
            e.Property(t => t.Id).HasMaxLength(64);
            e.Property(t => t.Identifier).HasMaxLength(128);
            e.Property(t => t.Name).HasMaxLength(256);
            e.Property(t => t.ConnectionString).HasMaxLength(1024);
        });

        modelBuilder.Entity<BoundedContext>(e =>
        {
            e.HasIndex(bc => bc.Key).IsUnique();
            e.Property(bc => bc.Key).HasMaxLength(64);
            e.Property(bc => bc.Name).HasMaxLength(128);
        });

        modelBuilder.Entity<Feature>(e =>
        {
            e.HasIndex(f => f.Key).IsUnique();
            e.Property(f => f.Key).HasMaxLength(128);
            e.Property(f => f.Name).HasMaxLength(128);
            e.HasOne(f => f.BoundedContext).WithMany(bc => bc.Features).HasForeignKey(f => f.BoundedContextId);
        });

        modelBuilder.Entity<Plan>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(128);
            e.Property(p => p.MonthlyPrice).HasPrecision(18, 2);
            e.Property(p => p.YearlyPrice).HasPrecision(18, 2);
            e.Property(p => p.IsActive).HasDefaultValue(true);
            e.Property(p => p.ApiRateLimitPerMinute).HasDefaultValue(500);
            e.Property(p => p.TrialDays).HasDefaultValue(14);
        });

        modelBuilder.Entity<PlanBoundedContext>(e =>
        {
            e.HasOne(pbc => pbc.Plan).WithMany(p => p.PlanBoundedContexts).HasForeignKey(pbc => pbc.PlanId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(pbc => pbc.BoundedContext).WithMany(bc => bc.PlanBoundedContexts).HasForeignKey(pbc => pbc.BoundedContextId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlanFeature>(e =>
        {
            e.HasOne(pf => pf.PlanBoundedContext).WithMany(pbc => pbc.PlanFeatures).HasForeignKey(pf => pf.PlanBoundedContextId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(pf => pf.Feature).WithMany(f => f.PlanFeatures).HasForeignKey(pf => pf.FeatureId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TenantBoundedContext>(e =>
        {
            e.HasOne(tbc => tbc.Tenant).WithMany(t => t.TenantBoundedContexts).HasForeignKey(tbc => tbc.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(tbc => tbc.BoundedContext).WithMany(bc => bc.TenantBoundedContexts).HasForeignKey(tbc => tbc.BoundedContextId).OnDelete(DeleteBehavior.Restrict);
            e.Property(tbc => tbc.IsEnabled).HasDefaultValue(true);
        });

        modelBuilder.Entity<TenantFeatureOverride>(e =>
        {
            e.HasOne(tfo => tfo.Tenant).WithMany(t => t.TenantFeatureOverrides).HasForeignKey(tfo => tfo.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(tfo => tfo.Feature).WithMany(f => f.TenantFeatureOverrides).HasForeignKey(tfo => tfo.FeatureId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Subscription>(e =>
        {
            e.HasOne(s => s.Tenant).WithMany(t => t.Subscriptions).HasForeignKey(s => s.TenantId);
            e.HasOne(s => s.Plan).WithMany(p => p.Subscriptions).HasForeignKey(s => s.PlanId);
            e.Property(s => s.BillingCycle).HasMaxLength(32).HasDefaultValue("Monthly");
            e.Property(s => s.Status).HasDefaultValue(SubscriptionStatus.Trial);
            e.Property(s => s.StartDate).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<CustomerAccount>(e =>
        {
            e.HasOne(ca => ca.Tenant).WithMany(t => t.CustomerAccounts).HasForeignKey(ca => ca.TenantId);
            e.Property(ca => ca.Email).HasMaxLength(256);
            e.Property(ca => ca.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<BillingInvoice>(e =>
        {
            e.HasOne(bi => bi.Subscription).WithMany(s => s.BillingInvoices).HasForeignKey(bi => bi.SubscriptionId);
            e.Property(bi => bi.Amount).HasPrecision(18, 2);
            e.Property(bi => bi.Currency).HasMaxLength(8).HasDefaultValue("TND");
            e.Property(bi => bi.IssuedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<FeaturePermission>(e =>
        {
            e.HasOne(fp => fp.Feature).WithMany(f => f.FeaturePermissions).HasForeignKey(fp => fp.FeatureId);
            e.Property(fp => fp.PermissionKey).HasMaxLength(128);
            e.HasIndex(fp => new { fp.FeatureId, fp.PermissionKey }).IsUnique();
        });

        modelBuilder.Entity<FeatureEndpointTag>(e =>
        {
            e.HasOne(fet => fet.Feature).WithMany(f => f.FeatureEndpointTags).HasForeignKey(fet => fet.FeatureId);
            e.Property(fet => fet.EndpointTag).HasMaxLength(128);
        });

        modelBuilder.Entity<FeatureRoute>(e =>
        {
            e.HasOne(fr => fr.Feature).WithMany(f => f.FeatureRoutes).HasForeignKey(fr => fr.FeatureId);
            e.Property(fr => fr.RoutePattern).HasMaxLength(256);
        });

        modelBuilder.Entity<TenantTheme>(e =>
        {
            e.HasOne(tt => tt.Tenant).WithMany().HasForeignKey(tt => tt.TenantId);
            e.HasIndex(tt => tt.TenantId).IsUnique();
        });

        modelBuilder.Entity<AdminAuditLog>(e =>
        {
            e.Property(a => a.Action).HasMaxLength(128);
            e.Property(a => a.TargetType).HasMaxLength(128);
            e.Property(a => a.TargetId).HasMaxLength(128);
            e.Property(a => a.PerformedBy).HasMaxLength(256);
            e.HasIndex(a => a.Timestamp);
        });
    }
}
