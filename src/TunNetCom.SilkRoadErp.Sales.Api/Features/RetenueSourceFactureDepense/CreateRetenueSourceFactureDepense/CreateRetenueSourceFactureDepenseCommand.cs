using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.CreateRetenueSourceFactureDepense;

public record CreateRetenueSourceFactureDepenseCommand(
    int FactureDepenseId,
    string? NumTej,
    string? PdfContent) : IRequest<Result<int>>;
