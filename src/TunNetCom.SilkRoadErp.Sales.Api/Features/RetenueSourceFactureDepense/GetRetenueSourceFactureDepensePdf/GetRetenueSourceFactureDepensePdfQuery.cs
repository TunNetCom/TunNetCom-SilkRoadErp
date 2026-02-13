namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.GetRetenueSourceFactureDepensePdf;

public record GetRetenueSourceFactureDepensePdfQuery(int FactureDepenseId) : IRequest<Result<byte[]>>;
