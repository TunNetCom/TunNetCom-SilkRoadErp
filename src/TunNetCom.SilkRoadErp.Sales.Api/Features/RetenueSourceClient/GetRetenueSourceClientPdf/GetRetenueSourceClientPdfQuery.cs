namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.GetRetenueSourceClientPdf;

public record GetRetenueSourceClientPdfQuery(int NumFacture) : IRequest<Result<byte[]>>;


