using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.EfCore;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.MultiTenancy;

public class TenantQueryFilterExtensionsTest
{
    private sealed class TenantEntity : ITenantEntity
    {
        public int Id { get; set; }
        public string TenantId { get; set; } = TenantConstants.DefaultTenantId;
    }

    private sealed class NonTenantEntity
    {
        public int Id { get; set; }
    }

    private sealed class FakeTenantContext : ITenantContext
    {
        public FakeTenantContext(string tenantId, bool isMultiTenant)
        {
            TenantId = tenantId;
            IsMultiTenant = isMultiTenant;
        }

        public string TenantId { get; }
        public TenantInfo? CurrentTenant => null;
        public bool IsResolved => true;
        public bool IsMultiTenant { get; }
    }

    private abstract class TenantDbContextBase : DbContext
    {
        protected TenantDbContextBase(DbContextOptions options) : base(options) { }

        public DbSet<TenantEntity> TenantEntities => Set<TenantEntity>();
        public DbSet<NonTenantEntity> NonTenantEntities => Set<NonTenantEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyTenantQueryFilters(CreateTenantContext());
        }

        protected abstract ITenantContext CreateTenantContext();
    }

    private sealed class MultiTenantDbContext : TenantDbContextBase
    {
        public MultiTenantDbContext(DbContextOptions options) : base(options) { }

        protected override ITenantContext CreateTenantContext()
            => new FakeTenantContext("tenant-1", isMultiTenant: true);
    }

    private sealed class SingleTenantDbContext : TenantDbContextBase
    {
        public SingleTenantDbContext(DbContextOptions options) : base(options) { }

        protected override ITenantContext CreateTenantContext()
            => new FakeTenantContext(TenantConstants.DefaultTenantId, isMultiTenant: false);
    }

    private static DbContextOptions<TenantDbContextBase> CreateOptions()
    {
        return new DbContextOptionsBuilder<TenantDbContextBase>()
            .UseInMemoryDatabase(databaseName: $"TenantFilter_{Guid.NewGuid()}")
            .Options;
    }

    [Fact]
    public void ApplyTenantQueryFilters_WhenSingleTenant_ShouldNotApplyFilter()
    {
        using var context = new SingleTenantDbContext(CreateOptions());

        var entityType = context.Model.FindEntityType(typeof(TenantEntity));

        entityType.Should().NotBeNull();
        entityType!.GetQueryFilter().Should().BeNull();
    }

    [Fact]
    public void ApplyTenantQueryFilters_WhenMultiTenant_ShouldApplyFilterToTenantEntity()
    {
        using var context = new MultiTenantDbContext(CreateOptions());

        var entityType = context.Model.FindEntityType(typeof(TenantEntity));

        entityType.Should().NotBeNull();
        entityType!.GetQueryFilter().Should().NotBeNull();
    }

    [Fact]
    public void ApplyTenantQueryFilters_WhenMultiTenant_ShouldNotApplyFilterToNonTenantEntity()
    {
        using var context = new MultiTenantDbContext(CreateOptions());

        var entityType = context.Model.FindEntityType(typeof(NonTenantEntity));

        entityType.Should().NotBeNull();
        entityType!.GetQueryFilter().Should().BeNull();
    }

    [Fact]
    public async Task ApplyTenantQueryFilters_WhenMultiTenant_ShouldFilterQueriesByTenant()
    {
        using var context = new MultiTenantDbContext(CreateOptions());
        _ = context.TenantEntities.Add(new TenantEntity { Id = 1, TenantId = "tenant-1" });
        _ = context.TenantEntities.Add(new TenantEntity { Id = 2, TenantId = "tenant-2" });
        _ = context.SaveChanges();

        var entities = await context.TenantEntities.ToListAsync();

        entities.Should().ContainSingle().Which.TenantId.Should().Be("tenant-1");
    }

    [Fact]
    public async Task ApplyTenantQueryFilters_WhenSingleTenant_ShouldReturnAllEntities()
    {
        using var context = new SingleTenantDbContext(CreateOptions());
        _ = context.TenantEntities.Add(new TenantEntity { Id = 1, TenantId = "tenant-1" });
        _ = context.TenantEntities.Add(new TenantEntity { Id = 2, TenantId = "tenant-2" });
        _ = context.SaveChanges();

        var entities = await context.TenantEntities.ToListAsync();

        entities.Should().HaveCount(2);
    }
}
