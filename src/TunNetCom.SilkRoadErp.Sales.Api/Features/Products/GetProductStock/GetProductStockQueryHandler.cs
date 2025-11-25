using FluentResults;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductStock;

public class GetProductStockQueryHandler(
    IStockCalculationService _stockCalculationService,
    IActiveAccountingYearService _activeAccountingYearService,
    ILogger<GetProductStockQueryHandler> _logger)
    : IRequestHandler<GetProductStockQuery, Result<ProductStockResponse>>
{
    public async Task<Result<ProductStockResponse>> Handle(GetProductStockQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting stock for product: {RefProduit}", query.RefProduit);

        var activeYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
        if (!activeYearId.HasValue)
        {
            _logger.LogWarning("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        var stockResult = await _stockCalculationService.CalculateStockAsync(query.RefProduit, activeYearId.Value, cancellationToken);

        var response = new ProductStockResponse
        {
            Reference = stockResult.Reference,
            StockInitial = stockResult.StockInitial,
            TotalAchats = stockResult.TotalAchats,
            TotalVentes = stockResult.TotalVentes,
            StockCalcule = stockResult.StockCalcule,
            StockDisponible = stockResult.StockDisponible
        };

        return Result.Ok(response);
    }
}

