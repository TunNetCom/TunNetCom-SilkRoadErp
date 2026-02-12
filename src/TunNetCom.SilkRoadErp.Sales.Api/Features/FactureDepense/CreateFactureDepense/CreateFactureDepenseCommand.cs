namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.CreateFactureDepense;

public record CreateFactureDepenseCommand(
    int IdTiersDepenseFonctionnement,
    DateTime Date,
    string Description,
    decimal MontantTotal,
    int? AccountingYearId,
    string? DocumentBase64) : IRequest<Result<int>>;
