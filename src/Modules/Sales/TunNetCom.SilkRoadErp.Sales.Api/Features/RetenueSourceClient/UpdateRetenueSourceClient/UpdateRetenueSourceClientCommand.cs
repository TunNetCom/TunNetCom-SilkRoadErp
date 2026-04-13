using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.UpdateRetenueSourceClient;

public record UpdateRetenueSourceClientCommand(
    int NumFacture,
    string? NumTej,
    string? PdfContent) : IRequest<Result>;


