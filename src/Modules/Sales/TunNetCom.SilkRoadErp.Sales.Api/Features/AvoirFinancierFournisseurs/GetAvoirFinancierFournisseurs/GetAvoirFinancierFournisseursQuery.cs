namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.GetAvoirFinancierFournisseurs;

public record GetAvoirFinancierFournisseursQuery(int Num) : IRequest<Result<Contracts.AvoirFinancierFournisseurs.AvoirFinancierFournisseursResponse>>;

