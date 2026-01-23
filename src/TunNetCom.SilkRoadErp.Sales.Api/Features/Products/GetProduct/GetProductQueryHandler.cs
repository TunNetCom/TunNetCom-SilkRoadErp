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
    private const string _priceColumn = nameof(ProductResponse.Price);
    public async Task<PagedList<ProductResponse>> Handle(GetProductQuery getProductQuery, CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(Produit), getProductQuery.PageNumber, getProductQuery.PageSize);
        var productsQuery = _context.Produit
            .Include(t => t.SousFamilleProduit)
                .ThenInclude(sf => sf.FamilleProduit)
            .AsQueryable();

        // Appliquer les filtres sur les entités
        if (getProductQuery.FamilleProduitId.HasValue)
        {
            productsQuery = productsQuery.Where(t => t.SousFamilleProduit != null && t.SousFamilleProduit.FamilleProduitId == getProductQuery.FamilleProduitId.Value);
        }

        if (getProductQuery.SousFamilleProduitId.HasValue)
        {
            productsQuery = productsQuery.Where(t => t.SousFamilleProduitId == getProductQuery.SousFamilleProduitId.Value);
        }

        if (getProductQuery.Visibility.HasValue)
        {
            productsQuery = productsQuery.Where(t => t.Visibilite == getProductQuery.Visibility.Value);
        }

        var productsQueryProjected = productsQuery.Select(t =>
            new ProductResponse
            {
                Id = t.Id,
                Reference = t.Refe,
                Name = t.Nom,
                QteLimit = t.QteLimite,
                DiscountPourcentage = t.Remise,
                DiscountPourcentageOfPurchasing = t.RemiseAchat,
                VatRate = t.Tva,
                Price = t.Prix,
                PurchasingPrice = t.PrixAchat,
                Visibility = t.Visibilite,
                SousFamilleProduitId = t.SousFamilleProduitId,
                SousFamilleProduitNom = t.SousFamilleProduit != null ? t.SousFamilleProduit.Nom : null,
                Image1StoragePath = t.Image1StoragePath,
                Image2StoragePath = t.Image2StoragePath,
                Image3StoragePath = t.Image3StoragePath
            })
            .AsQueryable();

        if (!string.IsNullOrEmpty(getProductQuery.SearchKeyword))
        {
            productsQueryProjected = productsQueryProjected.Where(
                c => c.Reference.Contains(getProductQuery.SearchKeyword)
                || c.Name.Contains(getProductQuery.SearchKeyword));
        }

        if (getProductQuery.SortOrder != null && getProductQuery.SortProprety != null)
        {
            _logger.LogInformation("sorting products column : {column} order : {order}", getProductQuery.SortProprety, getProductQuery.SortOrder);
            productsQueryProjected = ApplySorting(productsQueryProjected, getProductQuery.SortProprety, getProductQuery.SortOrder);
        }

        var pagedProducts = await PagedList<ProductResponse>.ToPagedListAsync(
            productsQueryProjected,
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
                    product.QteEnRetourFournisseur = stock.QteEnRetourFournisseur;
                    product.QteEnReparation = stock.QteEnReparation;
                    product.StockReel = stock.StockReel;
                }
            }
        }

        // Appliquer les filtres sur le stock calculé (après l'enrichissement)
        var filteredItems = pagedProducts.Items.AsEnumerable();

        if (getProductQuery.StockLowOnly == true)
        {
            filteredItems = filteredItems.Where(p => p.StockCalcule.HasValue && p.StockCalcule.Value < p.QteLimit);
        }

        if (getProductQuery.StockCalculeMin.HasValue)
        {
            filteredItems = filteredItems.Where(p => p.StockCalcule.HasValue && p.StockCalcule.Value >= getProductQuery.StockCalculeMin.Value);
        }

        if (getProductQuery.StockCalculeMax.HasValue)
        {
            filteredItems = filteredItems.Where(p => p.StockCalcule.HasValue && p.StockCalcule.Value <= getProductQuery.StockCalculeMax.Value);
        }

        // Si des filtres sur le stock ont été appliqués, recalculer la pagination
        if (getProductQuery.StockLowOnly == true || getProductQuery.StockCalculeMin.HasValue || getProductQuery.StockCalculeMax.HasValue)
        {
            var filteredList = filteredItems.ToList();
            var totalCount = filteredList.Count;
            var currentPage = getProductQuery.PageNumber;
            
            // Appliquer la pagination manuelle sur la liste filtrée
            var paginatedItems = filteredList
                .Skip((currentPage - 1) * getProductQuery.PageSize)
                .Take(getProductQuery.PageSize)
                .ToList();

            return new PagedList<ProductResponse>(paginatedItems, totalCount, currentPage, getProductQuery.PageSize);
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
            _ => query
        };
    }
}
