namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.DeleteProvider;
public class DeleteProviderCommand : IRequest<Result>

{
    public int Id { get; }
    public DeleteProviderCommand(int id)
    {
    Id = id;
    }

    
}
