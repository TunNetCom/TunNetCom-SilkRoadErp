using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Resolvers;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.MultiTenancy;

public class TenantResolversTest
{
    private static DefaultHttpContext CreateContext()
    {
        return new DefaultHttpContext();
    }

    [Fact]
    public void HeaderTenantResolver_Priority_ShouldBeTwo()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var resolver = new HeaderTenantResolver(config);

        resolver.Priority.Should().Be(2);
    }

    [Fact]
    public async Task HeaderTenantResolver_WhenHeaderPresent_ShouldReturnValue()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MultiTenancy:TenantResolution:Header:HeaderName"] = "X-Tenant-Id"
            })
            .Build();
        var resolver = new HeaderTenantResolver(config);
        var context = CreateContext();
        context.Request.Headers["X-Tenant-Id"] = "tenant-1";

        var result = await resolver.ResolveAsync(context);

        result.Should().Be("tenant-1");
    }

    [Fact]
    public async Task HeaderTenantResolver_WhenHeaderMissing_ShouldReturnNull()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var resolver = new HeaderTenantResolver(config);
        var context = CreateContext();

        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task HeaderTenantResolver_WhenHeaderWhitespace_ShouldReturnNull()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var resolver = new HeaderTenantResolver(config);
        var context = CreateContext();
        context.Request.Headers["X-Tenant-Id"] = "   ";

        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task HeaderTenantResolver_WhenHeaderEmptyString_ShouldReturnNull()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var resolver = new HeaderTenantResolver(config);
        var context = CreateContext();
        context.Request.Headers["X-Tenant-Id"] = string.Empty;

        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    [Fact]
    public void JwtClaimTenantResolver_Priority_ShouldBeThree()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var resolver = new JwtClaimTenantResolver(config);

        resolver.Priority.Should().Be(3);
    }

    [Fact]
    public async Task JwtClaimTenantResolver_WhenClaimPresent_ShouldReturnValue()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MultiTenancy:TenantResolution:JwtClaim:ClaimType"] = "tenant_id"
            })
            .Build();
        var resolver = new JwtClaimTenantResolver(config);
        var context = CreateContext();
        context.User = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim("tenant_id", "tenant-jwt")
            }));

        var result = await resolver.ResolveAsync(context);

        result.Should().Be("tenant-jwt");
    }

    [Fact]
    public async Task JwtClaimTenantResolver_WhenNoUser_ShouldReturnNull()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var resolver = new JwtClaimTenantResolver(config);
        var context = CreateContext();

        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task JwtClaimTenantResolver_WhenClaimWhitespace_ShouldReturnNull()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var resolver = new JwtClaimTenantResolver(config);
        var context = CreateContext();
        context.User = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim("tenant_id", "  ")
            }));

        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    [Fact]
    public void SubdomainTenantResolver_Priority_ShouldBeOne()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var resolver = new SubdomainTenantResolver(config);

        resolver.Priority.Should().Be(1);
    }

    [Fact]
    public async Task SubdomainTenantResolver_WhenBaseDomainMissing_ShouldReturnNull()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var resolver = new SubdomainTenantResolver(config);
        var context = CreateContext();
        context.Request.Host = new HostString("tenant1.example.com");

        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SubdomainTenantResolver_WhenHostMatches_ShouldReturnSubdomain()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MultiTenancy:TenantResolution:Subdomain:BaseDomain"] = "example.com"
            })
            .Build();
        var resolver = new SubdomainTenantResolver(config);
        var context = CreateContext();
        context.Request.Host = new HostString("tenant1.example.com");

        var result = await resolver.ResolveAsync(context);

        result.Should().Be("tenant1");
    }

    [Fact]
    public async Task SubdomainTenantResolver_WhenHostEqualsBaseDomain_ShouldReturnNull()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MultiTenancy:TenantResolution:Subdomain:BaseDomain"] = "example.com"
            })
            .Build();
        var resolver = new SubdomainTenantResolver(config);
        var context = CreateContext();
        context.Request.Host = new HostString("example.com");

        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SubdomainTenantResolver_WhenHostDoesNotMatch_ShouldReturnNull()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MultiTenancy:TenantResolution:Subdomain:BaseDomain"] = "example.com"
            })
            .Build();
        var resolver = new SubdomainTenantResolver(config);
        var context = CreateContext();
        context.Request.Host = new HostString("other.org");

        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SubdomainTenantResolver_WhenBaseDomainCaseDiffers_ShouldMatchIgnoringCase()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MultiTenancy:TenantResolution:Subdomain:BaseDomain"] = "EXAMPLE.com"
            })
            .Build();
        var resolver = new SubdomainTenantResolver(config);
        var context = CreateContext();
        context.Request.Host = new HostString("tenant1.example.com");

        var result = await resolver.ResolveAsync(context);

        result.Should().Be("tenant1");
    }
}
