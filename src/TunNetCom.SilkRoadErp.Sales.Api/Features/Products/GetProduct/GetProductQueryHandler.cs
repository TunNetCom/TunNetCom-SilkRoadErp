using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProduct;
public class GetProductQueryHandler(
    SalesContext _context,
    IStockCalculationService _stockCalculationService,
    IActiveAccountingYearService _activeAccountingYearService,
    ILogger<GetProductQueryHandler> _logger)
    : IRequestHandler<GetProductQuery, PagedList<ProductResponse>>
{
    private const string _referenceColumn = nameof(ProductResponse.Reference);
    private const string _qteColumn = nameof(ProductResponse.Qte);
    private const string _priceColumn = nameof(ProductResponse.Price);
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

        if (getProductQuery.SortOrder != null && getProductQuery.SortProprety != null)
        {
            _logger.LogInformation("sorting products column : {column} order : {order}", getProductQuery.SortProprety, getProductQuery.SortOrder);
            productsQuery = ApplySorting(productsQuery, getProductQuery.SortProprety, getProductQuery.SortOrder);
        }

        var pagedProducts = await PagedList<ProductResponse>.ToPagedListAsync(
            productsQuery,
            getProductQuery.PageNumber,
            getProductQuery.PageSize,
            cancellationToken);

        // Enrichir avec le stock calculé
        var activeYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
        if (activeYearId.HasValue)
        {
            var refProduits = pagedProducts.Items.Select(p => p.Reference).ToList();
            var stocks = await _stockCalculationService.CalculateStocksAsync(refProduits, activeYearId.Value, cancellationToken);

            foreach (var product in pagedProducts.Items)
            {
                if (stocks.TryGetValue(product.Reference, out var stock))
                {
                    product.StockCalcule = stock.StockCalcule;
                    product.StockDisponible = stock.StockDisponible;
                    product.IsStockLow = stock.StockDisponible <= product.QteLimit;
                }
            }
        }

        _logger.LogEntitiesFetched(nameof(Produit), pagedProducts.Items.Count);

        return pagedProducts;

    }
    private IQueryable<ProductResponse> ApplySorting(
        IQueryable<ProductResponse> productQuery,
        string sortProperty,
        string sortOrder)
    {
        return SortQuery(productQuery, sortProperty, sortOrder);
    }

    private IQueryable<ProductResponse> SortQuery(
        IQueryable<ProductResponse> query,
        string property,
        string order)
    {
        return (property, order) switch
        {
            (_referenceColumn, SortConstants.Ascending) => query.OrderBy(d => d.Reference),
            (_referenceColumn, SortConstants.Descending) => query.OrderByDescending(d => d.Reference),
            (_priceColumn, SortConstants.Ascending) => query.OrderBy(d => d.Price),
            (_priceColumn, SortConstants.Descending) => query.OrderByDescending(d => d.Price),
            (_qteColumn, SortConstants.Ascending) => query.OrderBy(d => d.Qte),
            (_qteColumn, SortConstants.Descending) => query.OrderByDescending(d => d.Qte),
            _ => query
        };
    }
}
