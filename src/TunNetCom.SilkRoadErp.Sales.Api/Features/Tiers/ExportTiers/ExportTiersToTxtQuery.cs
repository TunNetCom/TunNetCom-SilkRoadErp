namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tiers.ExportTiers;

public enum TierType
{
    Client,
    Supplier
}

public record ExportTiersToTxtQuery(TierType? Type = null) : IRequest<byte[]>;
