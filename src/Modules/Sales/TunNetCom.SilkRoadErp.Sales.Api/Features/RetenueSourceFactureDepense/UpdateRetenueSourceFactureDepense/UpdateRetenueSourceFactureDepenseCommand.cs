using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.UpdateRetenueSourceFactureDepense;

public record UpdateRetenueSourceFactureDepenseCommand(
    int FactureDepenseId,
    string? NumTej,
    string? PdfContent) : IRequest<Result>;
