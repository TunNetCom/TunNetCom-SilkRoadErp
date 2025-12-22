namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class ActiveAccountingYearService : IActiveAccountingYearService
{
    private readonly SalesContext _context;
    private readonly ILogger<ActiveAccountingYearService> _logger;
    private int? _cachedActiveYearId;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public ActiveAccountingYearService(
        SalesContext context,
        ILogger<ActiveAccountingYearService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int?> GetActiveAccountingYearIdAsync(CancellationToken cancellationToken = default)
    {
        // Si déjà en cache, retourner la valeur
        if (_cachedActiveYearId.HasValue)
        {
            return _cachedActiveYearId.Value;
        }

        // Utiliser un semaphore pour éviter les accès concurrents
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check après avoir acquis le verrou
            if (_cachedActiveYearId.HasValue)
            {
                return _cachedActiveYearId.Value;
            }

            // Récupérer l'exercice actif en ignorant les filtres de requête
            var activeYear = await _context.AccountingYear
                .IgnoreQueryFilters()
                .Where(ay => ay.IsActive)
                .Select(ay => new { ay.Id })
                .FirstOrDefaultAsync(cancellationToken);

            if (activeYear == null)
            {
                _logger.LogWarning("No active accounting year found");
                return null;
            }

            _cachedActiveYearId = activeYear.Id;
            return _cachedActiveYearId.Value;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public int? GetActiveAccountingYearId()
    {
        // Retourner la valeur mise en cache si disponible
        // Cette méthode est utilisée par les Global Query Filters qui ne supportent pas async
        return _cachedActiveYearId;
    }

    public void InvalidateCache()
    {
        _logger.LogInformation("Invalidating active accounting year cache");
        _cachedActiveYearId = null;
    }
}

