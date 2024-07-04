namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClientById;

public class GetClientByIdQuery : IRequest<Result<ClientResponse>>
{
    public int Id { get; set; }

    public GetClientByIdQuery(int id)
    {
        Id = id;
    }
}
