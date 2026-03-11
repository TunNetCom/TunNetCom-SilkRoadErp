using FluentAssertions;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;
using Xunit;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Tests;

public class MultiTenantContextTests
{
    [Fact]
    public void InitialState_IsNotResolved()
    {
        var context = new MultiTenantContext();

        context.IsResolved.Should().BeFalse();
        context.IsMultiTenant.Should().BeTrue();
        context.TenantId.Should().BeEmpty();
        context.CurrentTenant.Should().BeNull();
    }

    [Fact]
    public void SetTenant_PopulatesAllProperties()
    {
        var context = new MultiTenantContext();
        var tenantInfo = new TenantInfo
        {
            Id = "tenant-123",
            Identifier = "acme",
            Name = "Acme Corp",
            Strategy = TenancyStrategy.SharedDatabaseSharedSchema,
            ConnectionString = "Server=.;Database=Acme"
        };

        context.SetTenant(tenantInfo);

        context.IsResolved.Should().BeTrue();
        context.TenantId.Should().Be("tenant-123");
        context.CurrentTenant.Should().Be(tenantInfo);
        context.CurrentTenant!.Identifier.Should().Be("acme");
    }

    [Fact]
    public void SetTenant_ThrowsOnNull()
    {
        var context = new MultiTenantContext();

        var act = () => context.SetTenant(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
