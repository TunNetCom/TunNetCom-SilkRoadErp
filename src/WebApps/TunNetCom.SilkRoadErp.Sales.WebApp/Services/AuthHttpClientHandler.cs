using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public class AuthHttpClientHandler : DelegatingHandler
{
    private readonly IAuthService _authService;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AuthHttpClientHandler> _logger;
    private readonly IAutoLogoutService _autoLogoutService;
    private const string AccessTokenKey = "auth_access_token";

    public AuthHttpClientHandler(
        IAuthService authService,
        IJSRuntime jsRuntime,
        ILogger<AuthHttpClientHandler> logger,
        IAutoLogoutService autoLogoutService)
    {
        _authService = authService;
        _jsRuntime = jsRuntime;
        _logger = logger;
        _autoLogoutService = autoLogoutService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
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
                try
                {
                    var loadValueTask = _jsRuntime.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);
                    var loadTask = loadValueTask.AsTask();
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    
                    var completedTask = await Task.WhenAny(loadTask, timeoutTask);
                    
                    if (completedTask == timeoutTask)
                    {
                        _logger.LogWarning("Timeout loading token from localStorage for {Method} {Uri}", request.Method, request.RequestUri);
                    }
                    else
                    {
                        token = await loadTask;
                        if (!string.IsNullOrEmpty(token) && _authService != null)
                        {
                            // Update AuthService for future requests in this circuit
                            _authService.SetAccessToken(token);
                            _logger.LogDebug("Token loaded from localStorage for {Method} {Uri}", request.Method, request.RequestUri);
                        }
                    }
                }
                catch (InvalidOperationException ex) when (
                    ex.Message.Contains("prerendering") || 
                    ex.Message.Contains("statically rendered") || 
                    ex.Message.Contains("JavaScript interop calls cannot be issued"))
                {
                    _logger.LogDebug("JS interop not available during prerendering for {Method} {Uri}", request.Method, request.RequestUri);
                }
                catch (JSDisconnectedException)
                {
                    _logger.LogDebug("JS circuit disconnected for {Method} {Uri}", request.Method, request.RequestUri);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug("Task cancelled while loading token for {Method} {Uri}", request.Method, request.RequestUri);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading token from localStorage for {Method} {Uri}", request.Method, request.RequestUri);
                }
            }

            // Add Bearer token to request if available
            // AUTO-LOGOUT DISABLED: No expiration check - token is always sent if it exists
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
            _logger.LogError(ex, "AuthHttpClientHandler: Unexpected error while processing request {Method} {Uri}", 
                request.Method, request.RequestUri);
        }

        var response = await base.SendAsync(request, cancellationToken);
        
        // REACTIVE CHECK: Handle 401 Unauthorized responses
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogError("AuthHttpClientHandler: ===== 401 UNAUTHORIZED DETECTED: {Method} {Uri} =====", request.Method, request.RequestUri);
            
            // Check if this is a large request (likely with images) - don't auto-logout for these
            // as they might fail due to timeout or size issues, not necessarily auth issues
            var isLargeRequest = request.Content != null && 
                                 request.Content.Headers.ContentLength.HasValue && 
                                 request.Content.Headers.ContentLength.Value > 1024 * 1024; // > 1MB
            
            if (isLargeRequest)
            {
                _logger.LogWarning("AuthHttpClientHandler: Large request (>{Size}MB) returned 401 - may be due to timeout/size, not auth. Attempting token refresh first.", 
                    request.Content.Headers.ContentLength.Value / (1024 * 1024));
                
                // Try to refresh token first before clearing
                try
                {
                    var refreshed = await _authService.RefreshTokenAsync();
                    if (refreshed)
                    {
                        _logger.LogInformation("AuthHttpClientHandler: Token refreshed successfully for large request");
                        // Don't clear token or logout - let the caller retry
                        return response;
                    }
                }
                catch (Exception refreshEx)
                {
                    _logger.LogWarning(refreshEx, "AuthHttpClientHandler: Token refresh failed for large request");
                }
            }
            
            // AUTO-LOGOUT DISABLED: Don't clear token or trigger logout on 401
            // Just log the error - let the API handle authentication
            _logger.LogWarning("AuthHttpClientHandler: Received 401 Unauthorized for {Method} {Uri} - auto-logout disabled", 
                request.Method, request.RequestUri);
        }
        
        return response;
    }

    // AUTO-LOGOUT DISABLED: Method removed - no automatic logout on token expiration
}

