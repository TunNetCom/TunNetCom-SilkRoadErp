namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.DeletePaiementClient;

public record DeletePaiementClientCommand(int Id) : IRequest<Result>;

