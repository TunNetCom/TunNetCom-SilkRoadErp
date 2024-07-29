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
                Refe = t.Refe,
                Nom = t.Nom,
                Qte = t.Qte,
                QteLimite = t.QteLimite,
                Remise = t.Remise,
                RemiseAchat = t.RemiseAchat,
                Tva = t.Tva,
                Prix = t.Prix,
                PrixAchat = t.PrixAchat,
                Visibilite = t.Visibilite,
                
            })
            .AsQueryable();
        if (!string.IsNullOrEmpty(getProductQuery.SearchKeyword))
        {
            productsQuery = productsQuery.Where(
                c => c.Refe.Contains(getProductQuery.SearchKeyword)
                || c.Nom.Contains(getProductQuery.SearchKeyword));
        }
        var pagedProducts = await PagedList<ProductResponse>.ToPagedListAsync(
            productsQuery,
            getProductQuery.PageNumber,
            getProductQuery.PageSize,
            cancellationToken);

        _logger.LogEntitiesFetched(nameof(Produit), pagedProducts.Count);

        return pagedProducts;

    }
}
