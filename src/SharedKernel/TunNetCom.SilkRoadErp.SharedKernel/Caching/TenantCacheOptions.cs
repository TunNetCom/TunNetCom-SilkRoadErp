namespace TunNetCom.SilkRoadErp.SharedKernel.Caching;

public sealed class TenantCacheOptions
{
    public TimeSpan? AbsoluteExpiration { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
}
