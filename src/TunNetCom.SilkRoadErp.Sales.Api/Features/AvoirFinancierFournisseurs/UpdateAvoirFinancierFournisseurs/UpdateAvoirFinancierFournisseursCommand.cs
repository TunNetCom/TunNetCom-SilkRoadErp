using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.UpdateAvoirFinancierFournisseurs;

public record UpdateAvoirFinancierFournisseursCommand(
    int Num,
    int NumSurPage,
    DateTime Date,
    string? Description,
    decimal TotTtc
) : IRequest<Result>;

