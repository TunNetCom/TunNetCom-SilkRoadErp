using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.EfCore;
using TunNetCom.SilkRoadErp.Sales.UnitTests.Tests;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.MultiTenancy;

public class TenantSaveChangesInterceptorTest
{
    private sealed class TenantEntity : ITenantEntity
    {
        public int Id { get; set; }
        public string TenantId { get; set; } = TenantConstants.DefaultTenantId;
    }

    private sealed class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions options) : base(options) { }

        public DbSet<TenantEntity> Entities => Set<TenantEntity>();
    }

    private static Mock<ITenantContext> CreateContext(string tenantId, bool isMultiTenant = true)
    {
        var tenantContext = new Mock<ITenantContext>();
        tenantContext.Setup(x => x.TenantId).Returns(tenantId);
        tenantContext.Setup(x => x.IsMultiTenant).Returns(isMultiTenant);
        return tenantContext;
    }

    private static TenantDbContext CreateDbContext(Mock<ITenantContext> tenantContext)
    {
        var interceptor = new TenantSaveChangesInterceptor(
            tenantContext.Object,
            new TestLogger<TenantSaveChangesInterceptor>());

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(databaseName: $"TenantSave_{Guid.NewGuid()}")
            .AddInterceptors(interceptor)
            .Options;

        return new TenantDbContext(options);
    }

    [Fact]
    public void SaveChanges_WhenEntityAdded_ShouldSetTenantId()
    {
        using var context = CreateDbContext(CreateContext("tenant-1"));
        var entity = new TenantEntity();
        _ = context.Entities.Add(entity);

        _ = context.SaveChanges();

        entity.TenantId.Should().Be("tenant-1");
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityAdded_ShouldSetTenantId()
    {
        using var context = CreateDbContext(CreateContext("tenant-1"));
        var entity = new TenantEntity();
        _ = context.Entities.Add(entity);

        _ = await context.SaveChangesAsync();

        entity.TenantId.Should().Be("tenant-1");
    }

    [Fact]
    public void SaveChanges_WhenModifiedSameTenant_ShouldNotThrow()
    {
        using var context = CreateDbContext(CreateContext("tenant-1"));
        var entity = new TenantEntity { Id = 1, TenantId = "tenant-1" };
        _ = context.Entities.Add(entity);
        _ = context.SaveChanges();
        context.Entry(entity).State = EntityState.Modified;

        var act = () => context.SaveChanges();

        act.Should().NotThrow();
    }

    [Fact]
    public void SaveChanges_WhenModifiedCrossTenant_ShouldThrow()
    {
        using var context = CreateDbContext(CreateContext("tenant-2"));
        var entity = new TenantEntity { Id = 1, TenantId = "tenant-1" };
        _ = context.Entities.Add(entity);
        _ = context.SaveChanges();
        entity.TenantId = "tenant-1";
        context.Entry(entity).State = EntityState.Modified;

        var act = () => context.SaveChanges();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cross-tenant data modification is not allowed.");
    }

    [Fact]
    public void SaveChanges_WhenModifiedSingleTenantMode_ShouldNotCheckCrossTenant()
    {
        using var context = CreateDbContext(CreateContext("tenant-2", isMultiTenant: false));
        var entity = new TenantEntity { Id = 1, TenantId = "tenant-1" };
        _ = context.Entities.Add(entity);
        _ = context.SaveChanges();
        context.Entry(entity).State = EntityState.Modified;

        var act = () => context.SaveChanges();

        act.Should().NotThrow();
    }
}
