using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public interface IDecimalFormatService
{
    Task<string> GetDecimalFormatAsync();
    Task<int> GetDecimalPlacesAsync();
    string FormatAmount(decimal value);
    Task<string> FormatAmountAsync(decimal value);
}

public class DecimalFormatService : IDecimalFormatService
{
    private readonly IAppParametersClient _appParametersClient;
    private GetAppParametersResponse? _cachedParameters;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public DecimalFormatService(IAppParametersClient appParametersClient)
    {
        _appParametersClient = appParametersClient;
    }

    public async Task<string> GetDecimalFormatAsync()
    {
        var decimalPlaces = await GetDecimalPlacesAsync();
        return AmountHelper.GetDecimalFormat(decimalPlaces);
    }

    public async Task<int> GetDecimalPlacesAsync()
    {
        if (_cachedParameters != null)
        {
            return _cachedParameters.DecimalPlaces;
        }

        await _semaphore.WaitAsync();
        try
        {
            if (_cachedParameters != null)
            {
                return _cachedParameters.DecimalPlaces;
            }

            var result = await _appParametersClient.GetAppParametersAsync(CancellationToken.None);
            if (result.IsT0)
            {
                _cachedParameters = result.AsT0;
                return _cachedParameters.DecimalPlaces;
            }

            // Default to 3 if unable to fetch
            return AmountHelper.DEFAULT_DECIMAL_PLACES;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public string FormatAmount(decimal value)
    {
        // Use cached value if available, otherwise default to 3
        var decimalPlaces = _cachedParameters?.DecimalPlaces ?? AmountHelper.DEFAULT_DECIMAL_PLACES;
        return value.FormatAmount(decimalPlaces);
    }

    public async Task<string> FormatAmountAsync(decimal value)
    {
        var decimalPlaces = await GetDecimalPlacesAsync();
        return value.FormatAmount(decimalPlaces);
    }

    public void InvalidateCache()
    {
        _cachedParameters = null;
    }
}

