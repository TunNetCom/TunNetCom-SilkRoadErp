namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProduct;
public class GetProductQueryHandler(
    SalesContext _context,
    ILogger<GetProductQueryHandler> _logger)
    : IRequestHandler<GetProductQuery, PagedList<ProductResponse>>
{
    public async Task<PagedList<ProductResponse>> Handle(GetProductQuery getProductQuery, CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(Produit), getProductQuery.PageNumber, getProductQuery.PageSize);
        var productsQuery = _context.Produit.Select(t =>
            new ProductResponse
            {
                Reference = t.Refe,
                Name = t.Nom,
                Qte = t.Qte,
                QteLimit = t.QteLimite,
                DiscountPourcentage = t.Remise,
                DiscountPourcentageOfPurchasing = t.RemiseAchat,
                VatRate = t.Tva,
                Price = t.Prix,
                PurchasingPrice = t.PrixAchat,
                Visibility = t.Visibilite,
                
            })
            .AsQueryable();
        if (!string.IsNullOrEmpty(getProductQuery.SearchKeyword))
        {
            productsQuery = productsQuery.Where(
                c => c.Reference.Contains(getProductQuery.SearchKeyword)
                || c.Name.Contains(getProductQuery.SearchKeyword));
        }
        var pagedProducts = await PagedList<ProductResponse>.ToPagedListAsync(
            productsQuery,
            getProductQuery.PageNumber,
            getProductQuery.PageSize,
            cancellationToken);

        _logger.LogEntitiesFetched(nameof(Produit), pagedProducts.Items.Count);

        return pagedProducts;

    }
}
