using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.MultiTenancy;

public class TenantInfoTest
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var tenant = new TenantInfo
        {
            Id = "tenant-1",
            Identifier = "tenant1",
            Name = "Tenant 1",
            Strategy = TenancyStrategy.DatabasePerTenant,
            ConnectionString = "Server=.;Database=Tenant1",
            SchemaName = "schema",
            IsActive = false,
            Metadata = new Dictionary<string, string> { ["key"] = "value" }
        };

        tenant.Id.Should().Be("tenant-1");
        tenant.Identifier.Should().Be("tenant1");
        tenant.Name.Should().Be("Tenant 1");
        tenant.Strategy.Should().Be(TenancyStrategy.DatabasePerTenant);
        tenant.ConnectionString.Should().Be("Server=.;Database=Tenant1");
        tenant.SchemaName.Should().Be("schema");
        tenant.IsActive.Should().BeFalse();
        tenant.Metadata.Should().Contain("key", "value");
    }

    [Fact]
    public void Defaults_ShouldBeActiveWithEmptyMetadata()
    {
        var tenant = new TenantInfo
        {
            Id = "tenant-1",
            Identifier = "tenant1",
            Name = "Tenant 1",
            Strategy = TenancyStrategy.SharedDatabaseSharedSchema,
            ConnectionString = "conn"
        };

        tenant.IsActive.Should().BeTrue();
        tenant.SchemaName.Should().BeNull();
        tenant.Metadata.Should().BeEmpty();
    }
}

public class DeploymentOptionsTest
{
    [Fact]
    public void Default_ShouldBeStandalone()
    {
        var options = new DeploymentOptions();

        options.Mode.Should().Be(DeploymentMode.Standalone);
        options.DefaultTenantId.Should().Be(TenantConstants.DefaultTenantId);
        DeploymentOptions.SectionName.Should().Be("Deployment");
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var options = new DeploymentOptions
        {
            Mode = DeploymentMode.MultiTenant,
            DefaultTenantId = "custom"
        };

        options.Mode.Should().Be(DeploymentMode.MultiTenant);
        options.DefaultTenantId.Should().Be("custom");
    }
}
