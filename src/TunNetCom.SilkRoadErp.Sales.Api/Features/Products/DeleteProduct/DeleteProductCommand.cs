namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.DeleteProduct;
public class DeleteProductCommand : IRequest<Result>
{
    public string Refe { get; }
    public DeleteProductCommand(string refe)
    {
        Refe = refe;
    }
}
