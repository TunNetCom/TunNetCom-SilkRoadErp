using Microsoft.AspNetCore.Components.Server.Circuits;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

/// <summary>
/// Scoped service to track the current Blazor Server circuit ID
/// </summary>
public interface ICircuitIdService
{
    string GetCircuitId();
}

public class CircuitIdService : ICircuitIdService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CircuitIdService> _logger;
    private string? _circuitId;

    public CircuitIdService(IHttpContextAccessor httpContextAccessor, ILogger<CircuitIdService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string GetCircuitId()
    {
        if (_circuitId != null)
        {
            return _circuitId;
        }

        // Try to get from HttpContext
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            // Use session ID if available, otherwise connection ID
            var sessionId = httpContext.Session?.Id ?? httpContext.Connection.Id;
            _circuitId = sessionId;
            _logger.LogDebug("CircuitIdService: Using HttpContext-based ID: {CircuitId}", _circuitId);
            return _circuitId;
        }

        // Fallback to a generated GUID (shouldn't happen in normal operation)
        _circuitId = Guid.NewGuid().ToString();
        _logger.LogWarning("CircuitIdService: No HttpContext available, using generated ID: {CircuitId}", _circuitId);
        return _circuitId;
    }
}

