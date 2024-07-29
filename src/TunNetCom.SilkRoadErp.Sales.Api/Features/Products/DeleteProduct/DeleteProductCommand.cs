namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.DeleteProduct;

public record DeleteProductCommand(string Refe) : IRequest<Result>;