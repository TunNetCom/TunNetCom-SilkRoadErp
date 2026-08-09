using TunNetCom.SilkRoadErp.SharedKernel.Caching;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.Caching;

public class TenantCacheOptionsTest
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var options = new TenantCacheOptions
        {
            AbsoluteExpiration = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        options.AbsoluteExpiration.Should().Be(TimeSpan.FromMinutes(5));
        options.SlidingExpiration.Should().Be(TimeSpan.FromMinutes(2));
    }

    [Fact]
    public void Default_ShouldBeNull()
    {
        var options = new TenantCacheOptions();

        options.AbsoluteExpiration.Should().BeNull();
        options.SlidingExpiration.Should().BeNull();
    }
}
