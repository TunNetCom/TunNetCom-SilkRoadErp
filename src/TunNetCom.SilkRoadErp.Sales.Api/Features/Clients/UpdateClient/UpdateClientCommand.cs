namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public class UpdateClientCommand : IRequest<ClientResponse>
{
    public int Id { get; set; }
    public UpdateClientRequest Request { get; set; }

    public UpdateClientCommand(int id, UpdateClientRequest request)
    {
        Id = id;
        Request = request;
    }
}
