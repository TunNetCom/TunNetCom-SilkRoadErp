namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductByRef;
public class GetProductByRefQueryHandler(SalesContext _context, ILogger<GetProductByRefQueryHandler> _logger) : IRequestHandler<GetProductByRefQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByRefQuery getProductByRefQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById("Product", getProductByRefQuery.Refe);
        var product = await _context.Produit.FindAsync(getProductByRefQuery.Refe);
        if (product == null)
        {
            _logger.LogEntityNotFound("Product", getProductByRefQuery.Refe);
            return Result.Fail("Product_not_found");
        }
        _logger.LogEntityFetchedById("Product", getProductByRefQuery.Refe);
        return product.Adapt<ProductResponse>();
    }
}
