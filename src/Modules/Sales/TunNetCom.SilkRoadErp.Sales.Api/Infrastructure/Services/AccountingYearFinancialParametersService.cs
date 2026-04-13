using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class AccountingYearFinancialParametersService : IAccountingYearFinancialParametersService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AccountingYearFinancialParametersService> _logger;
    private AccountingYearFinancialParameters? _cachedParameters;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AccountingYearFinancialParametersService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AccountingYearFinancialParametersService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task<decimal> GetTimbreAsync(decimal fallbackValue = 0, CancellationToken cancellationToken = default)
    {
        var parameters = await GetAllFinancialParametersAsync(cancellationToken);
        return parameters?.Timbre ?? fallbackValue;
    }

    public async Task<decimal> GetPourcentageFodecAsync(decimal fallbackValue = 0, CancellationToken cancellationToken = default)
    {
        var parameters = await GetAllFinancialParametersAsync(cancellationToken);
        return parameters?.PourcentageFodec ?? fallbackValue;
    }

    public async Task<double> GetPourcentageRetenuAsync(double fallbackValue = 0, CancellationToken cancellationToken = default)
    {
        var parameters = await GetAllFinancialParametersAsync(cancellationToken);
        return parameters?.PourcentageRetenu ?? fallbackValue;
    }

    public async Task<decimal> GetVatRate0Async(decimal fallbackValue = 0, CancellationToken cancellationToken = default)
    {
        var parameters = await GetAllFinancialParametersAsync(cancellationToken);
        return parameters?.VatRate0 ?? fallbackValue;
    }

    public async Task<decimal> GetVatRate7Async(decimal fallbackValue = 7, CancellationToken cancellationToken = default)
    {
        var parameters = await GetAllFinancialParametersAsync(cancellationToken);
        return parameters?.VatRate7 ?? fallbackValue;
    }

    public async Task<decimal> GetVatRate13Async(decimal fallbackValue = 13, CancellationToken cancellationToken = default)
    {
        var parameters = await GetAllFinancialParametersAsync(cancellationToken);
        return parameters?.VatRate13 ?? fallbackValue;
    }

    public async Task<decimal> GetVatRate19Async(decimal fallbackValue = 19, CancellationToken cancellationToken = default)
    {
        var parameters = await GetAllFinancialParametersAsync(cancellationToken);
        return parameters?.VatRate19 ?? fallbackValue;
    }

    public async Task<decimal> GetVatAmountAsync(decimal fallbackValue = 19, CancellationToken cancellationToken = default)
    {
        var parameters = await GetAllFinancialParametersAsync(cancellationToken);
        return parameters?.VatAmount ?? fallbackValue;
    }

    public async Task<decimal> GetSeuilRetenueSourceAsync(decimal fallbackValue = 1000, CancellationToken cancellationToken = default)
    {
        var parameters = await GetAllFinancialParametersAsync(cancellationToken);
        return parameters?.SeuilRetenueSource ?? fallbackValue;
    }

    public async Task<int> GetDecimalPlacesAsync(int fallbackValue = 3, CancellationToken cancellationToken = default)
    {
        var parameters = await GetAllFinancialParametersAsync(cancellationToken);
        return parameters?.DecimalPlaces ?? fallbackValue;
    }

    public async Task<AccountingYearFinancialParameters?> GetAllFinancialParametersAsync(CancellationToken cancellationToken = default)
    {
        // Si déjà en cache, retourner la valeur
        if (_cachedParameters != null)
        {
            _logger.LogDebug("AccountingYearFinancialParametersService: Returning cached financial parameters");
            return _cachedParameters;
        }

        // Utiliser un semaphore pour éviter les accès concurrents
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check après avoir acquis le verrou
            if (_cachedParameters != null)
            {
                _logger.LogDebug("AccountingYearFinancialParametersService: Returning cached financial parameters (double-check)");
                return _cachedParameters;
            }

            _logger.LogInformation("AccountingYearFinancialParametersService: Cache miss, loading financial parameters from database");

            // Créer un scope pour obtenir le DbContext (qui est Scoped)
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SalesContext>();
            
            // Récupérer l'exercice actif avec les paramètres financiers migrés
            var activeYear = await context.AccountingYear
                .IgnoreQueryFilters()
                .Where(ay => ay.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (activeYear == null)
            {
                _logger.LogWarning("AccountingYearFinancialParametersService: No active accounting year found in database");
                return null;
            }

            // Récupérer DecimalPlaces depuis Systeme (non migré)
            var systeme = await context.Systeme
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            _cachedParameters = new AccountingYearFinancialParameters
            {
                Timbre = activeYear.Timbre ?? 0,
                PourcentageFodec = activeYear.PourcentageFodec ?? 0,
                PourcentageRetenu = activeYear.PourcentageRetenu ?? 0,
                VatRate0 = activeYear.VatRate0 ?? 0,
                VatRate7 = activeYear.VatRate7 ?? 7,
                VatRate13 = activeYear.VatRate13 ?? 13,
                VatRate19 = activeYear.VatRate19 ?? 19,
                VatAmount = activeYear.VatAmount ?? 19,
                SeuilRetenueSource = activeYear.SeuilRetenueSource ?? 1000,
                DecimalPlaces = systeme?.DecimalPlaces ?? 3
            };
            _logger.LogInformation("AccountingYearFinancialParametersService: Loaded and cached financial parameters");
            return _cachedParameters;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void InvalidateCache()
    {
        _logger.LogInformation("Invalidating accounting year financial parameters cache");
        _cachedParameters = null;
    }
}

