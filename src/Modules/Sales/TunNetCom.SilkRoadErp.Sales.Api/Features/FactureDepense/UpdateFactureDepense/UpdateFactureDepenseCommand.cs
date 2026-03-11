using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.UpdateFactureDepense;

public record UpdateFactureDepenseCommand(
    int Id,
    DateTime Date,
    string Description,
    decimal MontantTotal,
    List<FactureDepenseLigneTvaDto> LignesTVA,
    string? DocumentBase64) : IRequest<Result>;
