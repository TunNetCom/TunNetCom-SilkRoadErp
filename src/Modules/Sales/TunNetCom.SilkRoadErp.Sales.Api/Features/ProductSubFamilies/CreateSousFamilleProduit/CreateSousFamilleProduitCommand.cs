namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.CreateSousFamilleProduit;

public record CreateSousFamilleProduitCommand(string Nom, int FamilleProduitId) : IRequest<Result<int>>;

