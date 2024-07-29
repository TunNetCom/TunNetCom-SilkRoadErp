namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductByRef;

public class GetProductByRefQueryHandler(
    SalesContext _context,
    ILogger<GetProductByRefQueryHandler> _logger)
    : IRequestHandler<GetProductByRefQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(
        GetProductByRefQuery getProductByRefQuery,
        CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(Produit), getProductByRefQuery.Refe);
        var product = await _context.Produit.FindAsync(getProductByRefQuery.Refe,cancellationToken);

        if (product is null)
        {
            _logger.LogEntityNotFound(nameof(Produit), getProductByRefQuery.Refe);
            return Result.Fail("product_not_found");
        }
        _logger.LogEntityFetchedById(nameof(Produit), getProductByRefQuery.Refe);

        return product.Adapt<ProductResponse>();
    }
}
