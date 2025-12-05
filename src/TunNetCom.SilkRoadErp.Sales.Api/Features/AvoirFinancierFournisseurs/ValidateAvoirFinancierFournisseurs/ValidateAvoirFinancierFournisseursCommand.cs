namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.ValidateAvoirFinancierFournisseurs;

public record ValidateAvoirFinancierFournisseursCommand(List<int> Ids) : IRequest<Result>;

