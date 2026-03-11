namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

/// <summary>
/// Scoped service to track the current Blazor Server circuit ID.
/// Uses HttpContext Session to maintain a stable circuit ID across requests.
/// Falls back to connection ID or generates a GUID if session is not available.
/// </summary>
public interface ICircuitIdService
{
    string GetCircuitId();
}

public class CircuitIdService : ICircuitIdService
{
    private const string SessionCircuitIdKey = "SilkRoad_CircuitId";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CircuitIdService> _logger;
    private string? _cachedCircuitId;
    private readonly object _lock = new();

    public CircuitIdService(IHttpContextAccessor httpContextAccessor, ILogger<CircuitIdService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string GetCircuitId()
    {
        // Return cached value if available (for performance within same request)
        if (_cachedCircuitId != null)
        {
            return _cachedCircuitId;
        }

        lock (_lock)
        {
            if (_cachedCircuitId != null)
            {
                return _cachedCircuitId;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            
            // Try to get or create a stable circuit ID from session
            if (httpContext?.Session != null)
            {
                try
                {
                    // Check if we already have a circuit ID stored in session
                    var storedCircuitId = httpContext.Session.GetString(SessionCircuitIdKey);
                    if (!string.IsNullOrEmpty(storedCircuitId))
                    {
                        _cachedCircuitId = storedCircuitId;
                        _logger.LogDebug("CircuitIdService: Using session-stored circuit ID: {CircuitId}", 
                            _cachedCircuitId.Substring(0, Math.Min(8, _cachedCircuitId.Length)));
                        return _cachedCircuitId;
                    }

                    // Generate new circuit ID and store in session for future requests
                    _cachedCircuitId = Guid.NewGuid().ToString();
                    httpContext.Session.SetString(SessionCircuitIdKey, _cachedCircuitId);
                    _logger.LogInformation("CircuitIdService: Created new session-stored circuit ID: {CircuitId}", 
                        _cachedCircuitId.Substring(0, Math.Min(8, _cachedCircuitId.Length)));
                    return _cachedCircuitId;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "CircuitIdService: Could not access session, falling back to connection ID");
                }
            }

            // Fallback: Use connection ID if available
            if (httpContext?.Connection?.Id != null)
            {
                _cachedCircuitId = httpContext.Connection.Id;
                _logger.LogDebug("CircuitIdService: Using connection ID: {CircuitId}", 
                    _cachedCircuitId.Substring(0, Math.Min(8, _cachedCircuitId.Length)));
                return _cachedCircuitId;
            }

            // Last resort: Generate a GUID (not ideal as it won't persist across requests)
            _cachedCircuitId = Guid.NewGuid().ToString();
            _logger.LogWarning("CircuitIdService: No HttpContext available, generated fallback ID: {CircuitId}", 
                _cachedCircuitId.Substring(0, Math.Min(8, _cachedCircuitId.Length)));
            return _cachedCircuitId;
        }
    }
}

