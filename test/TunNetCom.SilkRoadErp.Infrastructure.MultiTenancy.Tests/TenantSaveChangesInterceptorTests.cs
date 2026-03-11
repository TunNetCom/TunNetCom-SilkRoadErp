using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.EfCore;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;
using Xunit;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Tests;

public class TenantSaveChangesInterceptorTests
{
    private class TestEntity : ITenantEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string TenantId { get; set; } = "default";
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<TestEntity> TestEntities => Set<TestEntity>();
    }

    [Fact]
    public async Task SavingChanges_SetsDefaultTenantId_InStandaloneMode()
    {
        var tenantContext = new StandaloneTenantContext(
            Options.Create(new DeploymentOptions { DefaultTenantId = "default" }));
        var logger = Mock.Of<ILogger<TenantSaveChangesInterceptor>>();
        var interceptor = new TenantSaveChangesInterceptor(tenantContext, logger);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase($"test-{Guid.NewGuid()}")
            .AddInterceptors(interceptor)
            .Options;

        using var db = new TestDbContext(options);
        db.TestEntities.Add(new TestEntity { Name = "Test" });
        await db.SaveChangesAsync();

        var entity = await db.TestEntities.FirstAsync();
        entity.TenantId.Should().Be("default");
    }

    [Fact]
    public async Task SavingChanges_SetsTenantId_InMultiTenantMode()
    {
        var tenantContext = new MultiTenantContext();
        tenantContext.SetTenant(new TenantInfo
        {
            Id = "tenant-abc",
            Identifier = "abc",
            Name = "ABC Corp",
            Strategy = TenancyStrategy.SharedDatabaseSharedSchema,
            ConnectionString = "test"
        });

        var logger = Mock.Of<ILogger<TenantSaveChangesInterceptor>>();
        var interceptor = new TenantSaveChangesInterceptor(tenantContext, logger);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase($"test-{Guid.NewGuid()}")
            .AddInterceptors(interceptor)
            .Options;

        using var db = new TestDbContext(options);
        db.TestEntities.Add(new TestEntity { Name = "Test" });
        await db.SaveChangesAsync();

        var entity = await db.TestEntities.FirstAsync();
        entity.TenantId.Should().Be("tenant-abc");
    }

    [Fact]
    public async Task SavingChanges_ThrowsOnCrossTenantWrite()
    {
        var tenantContext = new MultiTenantContext();
        tenantContext.SetTenant(new TenantInfo
        {
            Id = "tenant-abc",
            Identifier = "abc",
            Name = "ABC Corp",
            Strategy = TenancyStrategy.SharedDatabaseSharedSchema,
            ConnectionString = "test"
        });

        var logger = Mock.Of<ILogger<TenantSaveChangesInterceptor>>();
        var interceptor = new TenantSaveChangesInterceptor(tenantContext, logger);

        var dbName = $"test-{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(dbName)
            .AddInterceptors(interceptor)
            .Options;

        using var db = new TestDbContext(options);
        var entity = new TestEntity { Name = "Test", TenantId = "tenant-abc" };
        db.TestEntities.Add(entity);
        await db.SaveChangesAsync();

        entity.TenantId = "different-tenant";
        entity.Name = "Updated";
        db.Entry(entity).State = EntityState.Modified;

        var act = async () => await db.SaveChangesAsync();
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cross-tenant*");
    }
}
