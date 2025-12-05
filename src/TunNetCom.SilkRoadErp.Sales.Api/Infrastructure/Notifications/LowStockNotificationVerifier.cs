using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;

public class LowStockNotificationVerifier : INotificationVerifier
{
    private readonly SalesContext _context;
    private readonly ILogger<LowStockNotificationVerifier> _logger;
    private readonly IActiveAccountingYearService _activeAccountingYearService;
    private readonly IStockCalculationService _stockCalculationService;

    public LowStockNotificationVerifier(
        SalesContext context,
        ILogger<LowStockNotificationVerifier> logger,
        IActiveAccountingYearService activeAccountingYearService,
        IStockCalculationService stockCalculationService)
    {
        _context = context;
        _logger = logger;
        _activeAccountingYearService = activeAccountingYearService;
        _stockCalculationService = stockCalculationService;
    }

    public async Task<List<NotificationData>> VerifyAsync(CancellationToken cancellationToken)
    {
        var notifications = new List<NotificationData>();

        try
        {
            var accountingYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
            if (!accountingYearId.HasValue)
            {
                _logger.LogWarning("No active accounting year found, skipping low stock verification");
                return notifications;
            }

            // Get all products with their QteLimit
            var products = await _context.Produit
                .AsNoTracking()
                .Where(p => p.Visibilite) // Only visible products
                .Select(p => new { p.Refe, p.Nom, p.QteLimite })
                .ToListAsync(cancellationToken);

            if (!products.Any())
            {
                return notifications;
            }

            // Calculate stocks for all products
            var productReferences = products.Select(p => p.Refe).ToList();
            var stocks = await _stockCalculationService.CalculateStocksAsync(productReferences, accountingYearId.Value, cancellationToken);

            // Check for low stock
            foreach (var product in products)
            {
                if (stocks.TryGetValue(product.Refe, out var stock))
                {
                    if (stock.StockDisponible <= product.QteLimite)
                    {
                        notifications.Add(new NotificationData
                        {
                            Type = NotificationType.LowStock,
                            Title = $"Stock manquant: {product.Nom}",
                            Message = $"Le produit {product.Nom} (Réf: {product.Refe}) a un stock disponible de {stock.StockDisponible}, inférieur ou égal à la limite de {product.QteLimite}.",
                            RelatedEntityId = null, // We use product reference instead
                            RelatedEntityType = "Produit"
                        });
                    }
                }
            }

            _logger.LogInformation("LowStockNotificationVerifier found {Count} products with low stock", notifications.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying low stock");
        }

        return notifications;
    }
}

