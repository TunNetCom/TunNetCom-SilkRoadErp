using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public class AuthHttpClientHandler : DelegatingHandler
{
    private readonly IAuthService _authService;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AuthHttpClientHandler> _logger;
    private const string AccessTokenKey = "auth_access_token";
    
    // Track if we're already in a retry to prevent infinite loops
    private static readonly AsyncLocal<bool> _isRetrying = new();

    public AuthHttpClientHandler(
        IAuthService authService,
        IJSRuntime jsRuntime,
        ILogger<AuthHttpClientHandler> logger)
    {
        _authService = authService;
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Proactively ensure token is valid before making the request
        // This prevents unnecessary 401s when token is about to expire
        try
        {
            if (!_authService.IsTokenValid() && _authService.IsAuthenticated)
            {
                _logger.LogDebug("AuthHttpClientHandler: Token expiring soon, attempting proactive refresh for {Method} {Uri}", 
                    request.Method, request.RequestUri);
                await _authService.EnsureValidTokenAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AuthHttpClientHandler: Error during proactive token validation for {Method} {Uri}", 
                request.Method, request.RequestUri);
        }

        // Add token to the request
        await AddTokenToRequestAsync(request, cancellationToken);

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);
        
        // Handle 401 Unauthorized - try to refresh and retry ONCE
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !_isRetrying.Value)
        {
            _logger.LogWarning("AuthHttpClientHandler: 401 UNAUTHORIZED for {Method} {Uri} - attempting token refresh and retry", 
                request.Method, request.RequestUri);
            
            try
            {
                var refreshed = await _authService.RefreshTokenAsync();
                if (refreshed)
                {
                    _logger.LogInformation("AuthHttpClientHandler: Token refreshed successfully, retrying request");
                    
                    // Clone the request for retry (original request content may have been consumed)
                    var retryRequest = await CloneRequestAsync(request);
                    
                    // Add the new token
                    var newToken = _authService.AccessToken;
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                    }
                    
                    // Mark that we're retrying to prevent infinite loops
                    _isRetrying.Value = true;
                    try
                    {
                        // Dispose the original response before retrying
                        response.Dispose();
                        response = await base.SendAsync(retryRequest, cancellationToken);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("AuthHttpClientHandler: Retry successful for {Method} {Uri}", 
                                request.Method, request.RequestUri);
                        }
                        else
                        {
                            _logger.LogWarning("AuthHttpClientHandler: Retry still failed with {StatusCode} for {Method} {Uri}", 
                                response.StatusCode, request.Method, request.RequestUri);
                        }
                    }
                    finally
                    {
                        _isRetrying.Value = false;
                    }
                }
                else
                {
                    _logger.LogWarning("AuthHttpClientHandler: Token refresh failed for {Method} {Uri}", 
                        request.Method, request.RequestUri);
                }
            }
            catch (Exception refreshEx)
            {
                _logger.LogError(refreshEx, "AuthHttpClientHandler: Exception during token refresh for {Method} {Uri}", 
                    request.Method, request.RequestUri);
            }
        }
        
        return response;
    }

    private async Task AddTokenToRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            string? token = null;
            
            // STRATEGY: Try AuthService in-memory first (fastest), then localStorage (slower, may fail during prerendering)
            // This is important for Blazor Server where AuthService is scoped but persists within a user's circuit
            
            // 1. Try to get token from AuthService in-memory (works even during prerendering if set during login)
            token = _authService?.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogDebug("Token found in AuthService memory for {Method} {Uri}", request.Method, request.RequestUri);
            }
            else
            {
                // 2. Try to get token from localStorage (fallback)
                token = await TryLoadTokenFromLocalStorageAsync(request, cancellationToken);
            }

            // Add Bearer token to request if available
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogWarning("No token available for {Method} {Uri}", request.Method, request.RequestUri);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthHttpClientHandler: Unexpected error while adding token to request {Method} {Uri}", 
                request.Method, request.RequestUri);
        }
    }

    private async Task<string?> TryLoadTokenFromLocalStorageAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var loadValueTask = _jsRuntime.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);
            var loadTask = loadValueTask.AsTask();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            
            var completedTask = await Task.WhenAny(loadTask, timeoutTask);
            
            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("Timeout loading token from localStorage for {Method} {Uri}", request.Method, request.RequestUri);
                return null;
            }
            
            var token = await loadTask;
            if (!string.IsNullOrEmpty(token) && _authService != null)
            {
                // Update AuthService for future requests in this circuit
                _authService.SetAccessToken(token);
                _logger.LogDebug("Token loaded from localStorage for {Method} {Uri}", request.Method, request.RequestUri);
            }
            return token;
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("prerendering") || 
            ex.Message.Contains("statically rendered") || 
            ex.Message.Contains("JavaScript interop calls cannot be issued"))
        {
            _logger.LogDebug("JS interop not available during prerendering for {Method} {Uri}", request.Method, request.RequestUri);
            return null;
        }
        catch (JSDisconnectedException)
        {
            _logger.LogDebug("JS circuit disconnected for {Method} {Uri}", request.Method, request.RequestUri);
            return null;
        }
        catch (TaskCanceledException)
        {
            _logger.LogDebug("Task cancelled while loading token for {Method} {Uri}", request.Method, request.RequestUri);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading token from localStorage for {Method} {Uri}", request.Method, request.RequestUri);
            return null;
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);
        
        // Copy headers (except Authorization which we'll set with the new token)
        foreach (var header in request.Headers)
        {
            if (!header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }
        
        // Copy content if present
        if (request.Content != null)
        {
            // Read the content as bytes and create a new ByteArrayContent
            var contentBytes = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(contentBytes);
            
            // Copy content headers
            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }
        
        // Copy request properties/options
        foreach (var option in request.Options)
        {
            clone.Options.TryAdd(option.Key, option.Value);
        }
        
        return clone;
    }
}

