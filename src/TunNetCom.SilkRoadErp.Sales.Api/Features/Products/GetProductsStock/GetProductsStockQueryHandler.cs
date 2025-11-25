using FluentResults;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductsStock;

public class GetProductsStockQueryHandler(
    IStockCalculationService _stockCalculationService,
    IActiveAccountingYearService _activeAccountingYearService,
    ILogger<GetProductsStockQueryHandler> _logger)
    : IRequestHandler<GetProductsStockQuery, Result<List<ProductStockResponse>>>
{
    public async Task<Result<List<ProductStockResponse>>> Handle(GetProductsStockQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting stocks for {Count} products", query.RefProduits.Count);

        var activeYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
        if (!activeYearId.HasValue)
        {
            _logger.LogWarning("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        var stocksResult = await _stockCalculationService.CalculateStocksAsync(query.RefProduits, activeYearId.Value, cancellationToken);

        var response = stocksResult.Values.Select(s => new ProductStockResponse
        {
            Reference = s.Reference,
            StockInitial = s.StockInitial,
            TotalAchats = s.TotalAchats,
            TotalVentes = s.TotalVentes,
            StockCalcule = s.StockCalcule,
            StockDisponible = s.StockDisponible
        }).ToList();

        return Result.Ok(response);
    }
}

