namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductFamilies.UpdateFamilleProduit;

public record UpdateFamilleProduitCommand(int Id, string Nom) : IRequest<Result>;

