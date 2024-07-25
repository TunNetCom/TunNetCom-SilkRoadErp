namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.DeleteProvider;
public record DeleteProviderCommand : IRequest<Result>

{
    public int Id { get; }
    public DeleteProviderCommand(int id)
    { Id = id; }

}
