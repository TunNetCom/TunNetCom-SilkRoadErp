using System.Collections.Concurrent;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

/// <summary>
/// Singleton service to store JWT tokens per circuit
/// This allows tokens to be shared across all scoped services within a Blazor Server circuit
/// </summary>
public interface ITokenStore
{
    void SetToken(string circuitId, string token);
    string? GetToken(string circuitId);
    void ClearToken(string circuitId);
}

public class TokenStore : ITokenStore
{
    private readonly ConcurrentDictionary<string, string> _tokens = new();
    private readonly ILogger<TokenStore> _logger;

    public TokenStore(ILogger<TokenStore> logger)
    {
        _logger = logger;
    }

    public void SetToken(string circuitId, string token)
    {
        _tokens[circuitId] = token;
        _logger.LogInformation("TokenStore: Token set for circuit {CircuitId}. Token length: {Length}", 
            circuitId.Substring(0, Math.Min(8, circuitId.Length)), token.Length);
    }

    public string? GetToken(string circuitId)
    {
        if (_tokens.TryGetValue(circuitId, out var token))
        {
            _logger.LogDebug("TokenStore: Token retrieved for circuit {CircuitId}. Length: {Length}", 
                circuitId.Substring(0, Math.Min(8, circuitId.Length)), token.Length);
            return token;
        }
        
        _logger.LogWarning("TokenStore: No token found for circuit {CircuitId}", 
            circuitId.Substring(0, Math.Min(8, circuitId.Length)));
        return null;
    }

    public void ClearToken(string circuitId)
    {
        if (_tokens.TryRemove(circuitId, out _))
        {
            _logger.LogInformation("TokenStore: Token cleared for circuit {CircuitId}", 
                circuitId.Substring(0, Math.Min(8, circuitId.Length)));
        }
    }
}

