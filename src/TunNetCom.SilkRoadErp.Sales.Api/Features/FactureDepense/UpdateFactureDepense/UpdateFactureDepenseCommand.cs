namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.UpdateFactureDepense;

public record UpdateFactureDepenseCommand(
    int Id,
    DateTime Date,
    string Description,
    decimal MontantTotal) : IRequest<Result>;
