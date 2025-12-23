namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class ActiveAccountingYearService : IActiveAccountingYearService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ActiveAccountingYearService> _logger;
    private int? _cachedActiveYearId;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public ActiveAccountingYearService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ActiveAccountingYearService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task<int?> GetActiveAccountingYearIdAsync(CancellationToken cancellationToken = default)
    {
        // Si déjà en cache, retourner la valeur
        if (_cachedActiveYearId.HasValue)
        {
            _logger.LogDebug("ActiveAccountingYearService: Returning cached active year ID {ActiveYearId}", _cachedActiveYearId.Value);
            return _cachedActiveYearId.Value;
        }

        // Utiliser un semaphore pour éviter les accès concurrents
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check après avoir acquis le verrou
            if (_cachedActiveYearId.HasValue)
            {
                _logger.LogDebug("ActiveAccountingYearService: Returning cached active year ID {ActiveYearId} (double-check)", _cachedActiveYearId.Value);
                return _cachedActiveYearId.Value;
            }

            _logger.LogInformation("ActiveAccountingYearService: Cache miss, loading active year from database");

            // Créer un scope pour obtenir le DbContext (qui est Scoped)
            // Cela permet au service Singleton d'utiliser des services Scoped à la demande
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SalesContext>();
            
            // Récupérer l'exercice actif en ignorant les filtres de requête
            var activeYear = await context.AccountingYear
                .IgnoreQueryFilters()
                .Where(ay => ay.IsActive)
                .Select(ay => new { ay.Id })
                .FirstOrDefaultAsync(cancellationToken);

            if (activeYear == null)
            {
                _logger.LogWarning("ActiveAccountingYearService: No active accounting year found in database");
                return null;
            }

            _cachedActiveYearId = activeYear.Id;
            _logger.LogInformation("ActiveAccountingYearService: Loaded and cached active year ID {ActiveYearId}", activeYear.Id);
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

    public void SetActiveAccountingYearId(int? accountingYearId)
    {
        var oldValue = _cachedActiveYearId;
        _cachedActiveYearId = accountingYearId;
        
        _logger.LogInformation("ActiveAccountingYearService: Set active accounting year ID in cache from {OldValue} to {NewValue}", 
            oldValue, accountingYearId);
    }
}

