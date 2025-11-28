using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public class AuthHttpClientHandler : DelegatingHandler
{
    private readonly IAuthService _authService;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AuthHttpClientHandler> _logger;
    private const string AccessTokenKey = "auth_access_token";

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
        try
        {
            string? token = null;
            
            // STRATEGY: Try AuthService in-memory first (fastest), then localStorage (slower, may fail during prerendering)
            // This is important for Blazor Server where AuthService is scoped but persists within a user's circuit
            
            // 1. Try to get token from AuthService in-memory (works even during prerendering if set during login)
            token = _authService?.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("AuthHttpClientHandler: ✓ Token found in AuthService memory for request {Method} {Uri}. Length: {Length}", 
                    request.Method, request.RequestUri, token.Length);
            }
            else
            {
                _logger.LogWarning("AuthHttpClientHandler: AuthService has no token in memory for request {Method} {Uri}. Trying localStorage...", 
                    request.Method, request.RequestUri);
                
                // 2. Try to get token from localStorage (fallback)
                try
                {
                    var loadValueTask = _jsRuntime.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);
                    var loadTask = loadValueTask.AsTask();
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    
                    var completedTask = await Task.WhenAny(loadTask, timeoutTask);
                    
                    if (completedTask == timeoutTask)
                    {
                        _logger.LogError("AuthHttpClientHandler: ⏱️ TIMEOUT (5s) loading token from localStorage for request {Method} {Uri}", 
                            request.Method, request.RequestUri);
                    }
                    else
                    {
                        token = await loadTask;
                        if (!string.IsNullOrEmpty(token))
                        {
                            _logger.LogInformation("AuthHttpClientHandler: ✓ Token loaded from localStorage for request {Method} {Uri}. Length: {Length}", 
                                request.Method, request.RequestUri, token.Length);
                            
                            // Update AuthService for future requests in this circuit
                            if (_authService != null)
                            {
                                _authService.SetAccessToken(token);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("AuthHttpClientHandler: localStorage returned NULL or EMPTY token for key '{Key}'", AccessTokenKey);
                        }
                    }
                }
                catch (InvalidOperationException ex) when (
                    ex.Message.Contains("prerendering") || 
                    ex.Message.Contains("statically rendered") || 
                    ex.Message.Contains("JavaScript interop calls cannot be issued"))
                {
                    _logger.LogWarning("AuthHttpClientHandler: JS interop not available (prerendering) for request {Method} {Uri}. Token must be in AuthService memory or request will fail.", 
                        request.Method, request.RequestUri);
                }
                catch (JSDisconnectedException ex)
                {
                    _logger.LogWarning(ex, "AuthHttpClientHandler: JS circuit disconnected for request {Method} {Uri}", 
                        request.Method, request.RequestUri);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogWarning(ex, "AuthHttpClientHandler: Task cancelled while loading token for request {Method} {Uri}", 
                        request.Method, request.RequestUri);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AuthHttpClientHandler: Unexpected error loading token from localStorage for request {Method} {Uri}", 
                        request.Method, request.RequestUri);
                }
            }

            // Add Bearer token to request if available
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.LogInformation("AuthHttpClientHandler: ✓ Bearer token ADDED to request {Method} {Uri}. Token length: {Length}", 
                    request.Method, request.RequestUri, token.Length);
            }
            else
            {
                _logger.LogError("AuthHttpClientHandler: ✗ NO TOKEN available for request {Method} {Uri}. Request will return 401 if endpoint requires auth.", 
                    request.Method, request.RequestUri);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthHttpClientHandler: Unexpected error while processing request {Method} {Uri}", 
                request.Method, request.RequestUri);
        }

        var response = await base.SendAsync(request, cancellationToken);
        
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogError("AuthHttpClientHandler: ✗ Request {Method} {Uri} returned 401 Unauthorized", 
                request.Method, request.RequestUri);
        }
        
        return response;
    }
}

