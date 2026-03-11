namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.UpdateSousFamilleProduit;

public record UpdateSousFamilleProduitCommand(int Id, string Nom, int FamilleProduitId) : IRequest<Result>;

