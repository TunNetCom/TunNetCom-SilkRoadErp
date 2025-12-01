namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductFamilies.CreateFamilleProduit;

public record CreateFamilleProduitCommand(string Nom) : IRequest<Result<int>>;

