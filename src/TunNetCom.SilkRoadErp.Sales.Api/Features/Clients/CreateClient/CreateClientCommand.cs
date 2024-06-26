namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.CreateClient;

public class CreateClientCommand : IRequest<ClientResponse>
{
    public CreateClientRequest Request { get; set; }

    public CreateClientCommand(CreateClientRequest request)
    {
        Request = request;
    }
}
