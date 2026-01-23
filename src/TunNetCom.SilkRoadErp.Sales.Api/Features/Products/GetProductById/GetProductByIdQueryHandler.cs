namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductById;

public class GetProductByIdQueryHandler(
    SalesContext _context,
    ILogger<GetProductByIdQueryHandler> _logger)
    : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(Produit), query.Id);
        var product = await _context.Produit
            .FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (product is null)
        {
            _logger.LogEntityNotFound(nameof(Produit), query.Id);
            return Result.Fail(EntityNotFound.Error());
        }
        _logger.LogEntityFetchedById(nameof(Produit), query.Id);
        var productResponse = new ProductResponse
        {
            Id = product.Id,
            DiscountPourcentageOfPurchasing = product.RemiseAchat,
            DiscountPourcentage = product.Remise,
            Name = product.Nom,
            Price = product.Prix,
            PurchasingPrice = product.PrixAchat,
            QteLimit = product.QteLimite,
            Reference = product.Refe,
            VatRate = product.Tva,
            Visibility = product.Visibilite,
            SousFamilleProduitId = product.SousFamilleProduitId,
            Image1StoragePath = product.Image1StoragePath,
            Image2StoragePath = product.Image2StoragePath,
            Image3StoragePath = product.Image3StoragePath
        };
        return productResponse;
    }
}

