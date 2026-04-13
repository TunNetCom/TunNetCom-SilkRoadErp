using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.CreateRetenueSourceClient;

public record CreateRetenueSourceClientCommand(
    int NumFacture,
    string? NumTej,
    string? PdfContent) : IRequest<Result<int>>;


