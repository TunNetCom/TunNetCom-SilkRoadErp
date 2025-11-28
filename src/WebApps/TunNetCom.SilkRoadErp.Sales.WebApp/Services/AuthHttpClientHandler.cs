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
        
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized request: {Method} {Uri}", request.Method, request.RequestUri);
        }
        
        return response;
    }
}

