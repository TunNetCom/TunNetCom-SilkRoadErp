namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductByRef;
public class GetProductByRefQuery : IRequest<Result<ProductResponse>>
{ 
    public string? Refe { get; set; }
    public GetProductByRefQuery(string refe)
    {
        Refe = refe;
    }
}