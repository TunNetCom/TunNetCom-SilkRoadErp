using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Middleware;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Resolvers;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Store;
using TunNetCom.SilkRoadErp.Sales.UnitTests.Tests;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.MultiTenancy;

public class TenantResolutionMiddlewareTest
{
    private static TenantInfo CreateTenant(string id = "tenant-1", string identifier = "tenant1", bool isActive = true)
    {
        return new TenantInfo
        {
            Id = id,
            Identifier = identifier,
            Name = "Tenant 1",
            Strategy = TenancyStrategy.SharedDatabaseSharedSchema,
            ConnectionString = "conn",
            IsActive = isActive
        };
    }

    private static (HttpContext Context, MultiTenantContext TenantContext) CreateHttpContext(
        Mock<ITenantResolver>? resolver = null,
        Mock<ITenantStore>? store = null)
    {
        var services = new ServiceCollection();
        if (resolver != null)
        {
            services.AddScoped<ITenantResolver>(_ => resolver.Object);
        }
        if (store != null)
        {
            services.AddScoped(_ => store.Object);
        }
        var tenantContext = new MultiTenantContext();
        services.AddScoped(_ => tenantContext);
        var provider = services.BuildServiceProvider();

        var context = new DefaultHttpContext();
        context.RequestServices = provider;
        return (context, tenantContext);
    }

    private static TenantResolutionMiddleware CreateMiddleware()
    {
        return new TenantResolutionMiddleware(
            _ => Task.CompletedTask,
            new TestLogger<TenantResolutionMiddleware>());
    }

    [Fact]
    public async Task InvokeAsync_WhenNoResolverResolves_ShouldReturn400()
    {
        var resolver = new Mock<ITenantResolver>();
        resolver.Setup(x => x.Priority).Returns(1);
        resolver.Setup(x => x.ResolveAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync((string?)null);
        var store = new Mock<ITenantStore>();
        var (context, _) = CreateHttpContext(resolver, store);
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WhenTenantNotFound_ShouldReturn404()
    {
        var resolver = new Mock<ITenantResolver>();
        resolver.Setup(x => x.Priority).Returns(1);
        resolver.Setup(x => x.ResolveAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync("tenant1");
        var store = new Mock<ITenantStore>();
        store.Setup(x => x.GetByIdentifierAsync("tenant1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantInfo?)null);
        var (context, _) = CreateHttpContext(resolver, store);
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task InvokeAsync_WhenTenantInactive_ShouldReturn403()
    {
        var resolver = new Mock<ITenantResolver>();
        resolver.Setup(x => x.Priority).Returns(1);
        resolver.Setup(x => x.ResolveAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync("tenant1");
        var store = new Mock<ITenantStore>();
        store.Setup(x => x.GetByIdentifierAsync("tenant1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTenant(isActive: false));
        var (context, _) = CreateHttpContext(resolver, store);
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_WhenTenantResolved_ShouldSetTenantContextAndCallNext()
    {
        var resolver = new Mock<ITenantResolver>();
        resolver.Setup(x => x.Priority).Returns(1);
        resolver.Setup(x => x.ResolveAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync("tenant1");
        var store = new Mock<ITenantStore>();
        store.Setup(x => x.GetByIdentifierAsync("tenant1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTenant());
        var (context, tenantContext) = CreateHttpContext(resolver, store);
        var nextCalled = false;
        var middleware = new TenantResolutionMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            new TestLogger<TenantResolutionMiddleware>());

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        tenantContext.IsResolved.Should().BeTrue();
        tenantContext.TenantId.Should().Be("tenant-1");
    }

    [Fact]
    public async Task InvokeAsync_WhenMultipleResolvers_ShouldUseFirstThatResolves()
    {
        var first = new Mock<ITenantResolver>();
        first.Setup(x => x.Priority).Returns(1);
        first.Setup(x => x.ResolveAsync(It.IsAny<HttpContext>())).ReturnsAsync((string?)null);
        var second = new Mock<ITenantResolver>();
        second.Setup(x => x.Priority).Returns(2);
        second.Setup(x => x.ResolveAsync(It.IsAny<HttpContext>())).ReturnsAsync("tenant2");
        var store = new Mock<ITenantStore>();
        store.Setup(x => x.GetByIdentifierAsync("tenant2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTenant(id: "tenant-2", identifier: "tenant2"));

        var services = new ServiceCollection();
        services.AddScoped<ITenantResolver>(_ => first.Object);
        services.AddScoped<ITenantResolver>(_ => second.Object);
        services.AddScoped(_ => store.Object);
        var tenantContext = new MultiTenantContext();
        services.AddScoped(_ => tenantContext);
        var provider = services.BuildServiceProvider();
        var context = new DefaultHttpContext();
        context.RequestServices = provider;
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        tenantContext.TenantId.Should().Be("tenant-2");
    }
}
