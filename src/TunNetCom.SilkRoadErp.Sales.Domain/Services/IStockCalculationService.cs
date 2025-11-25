namespace TunNetCom.SilkRoadErp.Sales.Domain.Services;

public interface IStockCalculationService
{
    Task<ProductStockResult> CalculateStockAsync(string refProduit, int accountingYearId, CancellationToken cancellationToken = default);
    Task<Dictionary<string, ProductStockResult>> CalculateStocksAsync(List<string> refProduits, int accountingYearId, CancellationToken cancellationToken = default);
}

public class ProductStockResult
{
    public string Reference { get; set; } = string.Empty;
    public int StockInitial { get; set; }
    public int TotalAchats { get; set; }
    public int TotalVentes { get; set; }
    public int StockCalcule { get; set; }
    public int StockDisponible { get; set; }
}

