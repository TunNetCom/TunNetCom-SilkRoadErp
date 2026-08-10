using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Behaviors;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.MultiTenancy;

public class TenantValidationBehaviorTest
{
    private sealed record TestRequest : IRequest<Unit>;

    private static Mock<ITenantContext> CreateContext()
    {
        return new Mock<ITenantContext>();
    }

    [Fact]
    public async Task Handle_WhenSingleTenant_ShouldCallNext()
    {
        var tenantContext = CreateContext();
        tenantContext.Setup(x => x.IsMultiTenant).Returns(false);
        var behavior = new TenantValidationBehavior<TestRequest, Unit>(tenantContext.Object);
        var called = false;

        var result = await behavior.Handle(
            new TestRequest(),
            (ct) => { called = true; return Task.FromResult(Unit.Value); },
            CancellationToken.None);

        result.Should().Be(Unit.Value);
        called.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenNotResolved_ShouldThrow()
    {
        var tenantContext = CreateContext();
        tenantContext.Setup(x => x.IsMultiTenant).Returns(true);
        tenantContext.Setup(x => x.IsResolved).Returns(false);
        var behavior = new TenantValidationBehavior<TestRequest, Unit>(tenantContext.Object);

        var act = () => behavior.Handle(
            new TestRequest(),
            (ct) => Task.FromResult(Unit.Value),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Tenant context is not resolved*");
    }

    [Fact]
    public async Task Handle_WhenTenantInactive_ShouldThrow()
    {
        var tenantContext = CreateContext();
        tenantContext.Setup(x => x.IsMultiTenant).Returns(true);
        tenantContext.Setup(x => x.IsResolved).Returns(true);
        tenantContext.Setup(x => x.CurrentTenant).Returns(new TenantInfo
        {
            Id = "tenant-1",
            Identifier = "tenant1",
            Name = "Tenant 1",
            Strategy = TenancyStrategy.SharedDatabaseSharedSchema,
            ConnectionString = "conn",
            IsActive = false
        });
        tenantContext.Setup(x => x.TenantId).Returns("tenant-1");
        var behavior = new TenantValidationBehavior<TestRequest, Unit>(tenantContext.Object);

        var act = () => behavior.Handle(
            new TestRequest(),
            (ct) => Task.FromResult(Unit.Value),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*is not active*");
    }

    [Fact]
    public async Task Handle_WhenTenantActive_ShouldCallNext()
    {
        var tenantContext = CreateContext();
        tenantContext.Setup(x => x.IsMultiTenant).Returns(true);
        tenantContext.Setup(x => x.IsResolved).Returns(true);
        tenantContext.Setup(x => x.CurrentTenant).Returns(new TenantInfo
        {
            Id = "tenant-1",
            Identifier = "tenant1",
            Name = "Tenant 1",
            Strategy = TenancyStrategy.SharedDatabaseSharedSchema,
            ConnectionString = "conn",
            IsActive = true
        });
        var behavior = new TenantValidationBehavior<TestRequest, Unit>(tenantContext.Object);
        var called = false;

        var result = await behavior.Handle(
            new TestRequest(),
            (ct) => { called = true; return Task.FromResult(Unit.Value); },
            CancellationToken.None);

        result.Should().Be(Unit.Value);
        called.Should().BeTrue();
    }
}
