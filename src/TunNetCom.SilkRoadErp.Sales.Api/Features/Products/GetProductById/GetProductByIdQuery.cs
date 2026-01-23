namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductById;

public class GetProductByIdQuery : IRequest<Result<ProductResponse>>
{ 
    public int Id { get; set; }

    public GetProductByIdQuery(int id)
    {
        Id = id;
    }
}

