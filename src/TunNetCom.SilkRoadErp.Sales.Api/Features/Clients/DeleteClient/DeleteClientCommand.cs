namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.DeleteClient;

public class DeleteClientCommand : IRequest<Result>
{
    public int Id { get; }

    public DeleteClientCommand(int id)
    {
        Id = id;
    }
}
