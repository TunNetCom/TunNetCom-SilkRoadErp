using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.CreateAvoirFinancierFournisseurs;

public record CreateAvoirFinancierFournisseursCommand(
    int NumFactureFournisseur,
    int NumSurPage,
    DateTime Date,
    string? Description,
    decimal TotTtc
) : IRequest<Result<int>>;

