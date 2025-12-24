using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.JSInterop;
using TunNetCom.SilkRoadErp.Sales.Contracts.Auth;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    Task<bool> RefreshTokenAsync();
    Task LogoutAsync();
    Task LoadTokenFromStorageAsync();
    bool IsAuthenticated { get; }
    string? AccessToken { get; }
    void SetAccessToken(string? token);
    UserInfo? GetUserInfo();
    
    /// <summary>
    /// Checks if the token is expired or will expire soon (within 5 minutes).
    /// Returns true if token is valid and not expiring soon.
    /// </summary>
    bool IsTokenValid();
    
    /// <summary>
    /// Gets the token expiration time, or null if no token or invalid token.
    /// </summary>
    DateTime? GetTokenExpiration();
    
    /// <summary>
    /// Ensures the token is valid, refreshing it if necessary.
    /// Returns true if a valid token is available after the operation.
    /// </summary>
    Task<bool> EnsureValidTokenAsync();
}

public class UserInfo
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;
    private readonly IJSRuntime _jsRuntime;
    private readonly ITokenStore _tokenStore;
    private readonly ICircuitIdService _circuitIdService;
    private const string AccessTokenKey = "auth_access_token";
    private const string RefreshTokenKey = "auth_refresh_token";
    
    // Local cache for this scoped service instance
    // Each circuit will have its own isolated cache
    private string? _localAccessToken;

    public AuthService(
        HttpClient httpClient, 
        ILogger<AuthService> logger, 
        IJSRuntime jsRuntime,
        ITokenStore tokenStore,
        ICircuitIdService circuitIdService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsRuntime = jsRuntime;
        _tokenStore = tokenStore;
        _circuitIdService = circuitIdService;
        
        // DO load token from circuit-specific TokenStore on construction
        // This allows token to persist within the same circuit across service recreations
        try
        {
            var circuitId = _circuitIdService.GetCircuitId();
            _localAccessToken = _tokenStore.GetToken(circuitId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load token from TokenStore in constructor");
            _localAccessToken = null;
        }
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public string? AccessToken
    {
        get
        {
            // Only return the local cache value
            // Do NOT try to load from TokenStore here to avoid shared sessions
            
            return _localAccessToken;
        }
        private set
        {
            _localAccessToken = value;
            // Cache in TokenStore using circuit-specific key for performance only
            // This is optional - if it fails, token is still available in memory
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    var circuitId = _circuitIdService.GetCircuitId();
                    _tokenStore.SetToken(circuitId, value);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to cache token in TokenStore, but token is available in memory");
                }
            }
        }
    }
    
    // Public setter for AuthHttpClientHandler to set token directly (avoids timeout issues)
    public void SetAccessToken(string? token)
    {
        _localAccessToken = token;
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var circuitId = _circuitIdService.GetCircuitId();
                _tokenStore.SetToken(circuitId, token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to cache token in TokenStore, but token is available in memory");
            }
        }
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Login failed with status {StatusCode}", response.StatusCode);
                return false;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse == null)
            {
                _logger.LogWarning("Login response was null");
                return false;
            }

            if (string.IsNullOrEmpty(loginResponse.AccessToken))
            {
                _logger.LogError("Login response AccessToken is null or empty");
                return false;
            }

            // Set token in memory FIRST - this is critical for immediate use
            AccessToken = loginResponse.AccessToken;
            _logger.LogInformation("Login: Token set in memory. Length: {Length}", loginResponse.AccessToken.Length);
            
            // Then try to store in localStorage for persistence (may fail during prerendering, that's OK)
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, loginResponse.AccessToken);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, loginResponse.RefreshToken);
                _logger.LogInformation("Login: Tokens stored in localStorage successfully");
            }
            catch (InvalidOperationException ex) when (
                ex.Message.Contains("prerendering") || 
                ex.Message.Contains("statically rendered") || 
                ex.Message.Contains("JavaScript interop calls cannot be issued"))
            {
                _logger.LogWarning("Login: Cannot store in localStorage during prerendering, but token is available in memory");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Login: Failed to store tokens in localStorage, but token is available in memory");
            }
            
            _logger.LogInformation("Login successful for user {Username}. Token available in memory. Length: {Length}", 
                username, loginResponse.AccessToken.Length);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return false;
        }
    }

    public async Task LoadTokenFromStorageAsync()
    {
        try
        {
            _logger.LogDebug("LoadTokenFromStorageAsync: Attempting to load token from localStorage");
            
            // Use a timeout to prevent indefinite blocking
            var loadValueTask = _jsRuntime.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);
            var loadTask = loadValueTask.AsTask();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2));
            var completedTask = await Task.WhenAny(loadTask, timeoutTask);
            
            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("LoadTokenFromStorageAsync: Timeout loading token from localStorage");
                return;
            }
            
            var tokenFromStorage = await loadTask;
            
            if (!string.IsNullOrEmpty(tokenFromStorage))
            {
                // Set the token in local cache first (this is critical)
                _localAccessToken = tokenFromStorage;
                
                // Try to cache in circuit-specific TokenStore (optional, for performance)
                try
                {
                    var circuitId = _circuitIdService.GetCircuitId();
                    _tokenStore.SetToken(circuitId, tokenFromStorage);
                    _logger.LogInformation("LoadTokenFromStorageAsync: Token loaded successfully for circuit {CircuitId}. Length: {Length}", 
                        circuitId.Substring(0, Math.Min(8, circuitId.Length)), tokenFromStorage.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "LoadTokenFromStorageAsync: Could not cache in TokenStore, but token is loaded in memory. Length: {Length}", tokenFromStorage.Length);
                }
            }
            else
            {
                _logger.LogDebug("LoadTokenFromStorageAsync: No token found in localStorage");
            }
        }
        catch (JSDisconnectedException ex)
        {
            // Component was disposed or circuit disconnected, ignore
            _logger.LogDebug(ex, "LoadTokenFromStorageAsync: JSDisconnectedException - circuit disconnected");
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("prerendering") || 
            ex.Message.Contains("statically rendered") || 
            ex.Message.Contains("JavaScript interop calls cannot be issued"))
        {
            // JS interop not available during prerendering, this is expected
            _logger.LogDebug("LoadTokenFromStorageAsync: JS interop not available during prerendering");
        }
        catch (InvalidOperationException ex)
        {
            // Other JS interop errors
            _logger.LogWarning(ex, "LoadTokenFromStorageAsync: InvalidOperationException");
        }
        catch (TaskCanceledException)
        {
            _logger.LogDebug("LoadTokenFromStorageAsync: Task was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoadTokenFromStorageAsync: Unexpected error loading token");
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            _logger.LogInformation("RefreshTokenAsync: Starting token refresh");
            
            // Try to get refresh token from localStorage with timeout
            string? refreshToken = null;
            try
            {
                var loadValueTask = _jsRuntime.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);
                var loadTask = loadValueTask.AsTask();
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(3));
                var completedTask = await Task.WhenAny(loadTask, timeoutTask);
                
                if (completedTask == timeoutTask)
                {
                    _logger.LogWarning("RefreshTokenAsync: Timeout getting refresh token from localStorage");
                    return false;
                }
                
                refreshToken = await loadTask;
            }
            catch (InvalidOperationException ex) when (
                ex.Message.Contains("prerendering") || 
                ex.Message.Contains("statically rendered") || 
                ex.Message.Contains("JavaScript interop calls cannot be issued"))
            {
                _logger.LogDebug("RefreshTokenAsync: JS interop not available during prerendering");
                return false;
            }
            catch (JSDisconnectedException)
            {
                _logger.LogDebug("RefreshTokenAsync: JS circuit disconnected");
                return false;
            }
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("RefreshTokenAsync: No refresh token found in localStorage");
                return false;
            }

            _logger.LogInformation("RefreshTokenAsync: Refresh token found, calling API");

            var request = new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/refresh", request);
            
            _logger.LogInformation("RefreshTokenAsync: API response status: {StatusCode}", response.StatusCode);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("RefreshTokenAsync: Token refresh failed with status {StatusCode}", response.StatusCode);
                return false;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse == null)
            {
                _logger.LogWarning("RefreshTokenAsync: LoginResponse is null");
                return false;
            }

            if (string.IsNullOrEmpty(loginResponse.AccessToken))
            {
                _logger.LogWarning("RefreshTokenAsync: AccessToken in response is null or empty");
                return false;
            }

            _logger.LogInformation("RefreshTokenAsync: Updating tokens in localStorage and memory");

            // Update tokens in localStorage (with error handling for prerendering)
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, loginResponse.AccessToken);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, loginResponse.RefreshToken);
            }
            catch (InvalidOperationException ex) when (
                ex.Message.Contains("prerendering") || 
                ex.Message.Contains("statically rendered") || 
                ex.Message.Contains("JavaScript interop calls cannot be issued"))
            {
                _logger.LogWarning("RefreshTokenAsync: Cannot store tokens in localStorage during prerendering");
                // Continue anyway - at least set in memory
            }
            catch (JSDisconnectedException)
            {
                _logger.LogWarning("RefreshTokenAsync: JS circuit disconnected while storing tokens");
                // Continue anyway - at least set in memory
            }
            
            AccessToken = loginResponse.AccessToken;
            
            _logger.LogInformation("RefreshTokenAsync: ===== TOKEN REFRESH SUCCESSFUL =====");
            return true;
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("prerendering") || 
            ex.Message.Contains("statically rendered") || 
            ex.Message.Contains("JavaScript interop calls cannot be issued"))
        {
            _logger.LogDebug("RefreshTokenAsync: JS interop not available during prerendering");
            return false;
        }
        catch (JSDisconnectedException)
        {
            _logger.LogDebug("RefreshTokenAsync: JS circuit disconnected");
            return false;
        }
        catch (TaskCanceledException)
        {
            _logger.LogDebug("RefreshTokenAsync: Task was cancelled");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RefreshTokenAsync: ===== EXCEPTION during token refresh =====");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        _logger.LogInformation("Logout: Starting logout process");
        
        try
        {
            var refreshToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var request = new { RefreshToken = refreshToken };
                await _httpClient.PostAsJsonAsync("/api/auth/logout", request);
                _logger.LogInformation("Logout: Logout request sent to API");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout API call");
        }
        finally
        {
            // Clear token from local cache
            _localAccessToken = null;
            _logger.LogInformation("Logout: Token cleared from local cache");
            
            // Try to clear token from circuit-specific TokenStore cache (optional)
            try
            {
                var circuitId = _circuitIdService.GetCircuitId();
                _tokenStore.ClearToken(circuitId);
                _logger.LogInformation("Logout: Token cleared from TokenStore for circuit {CircuitId}", 
                    circuitId.Substring(0, Math.Min(8, circuitId.Length)));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Logout: Could not clear TokenStore, but local cache is cleared");
            }
            
            // Clear tokens from localStorage
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
                _logger.LogInformation("Logout: Tokens cleared from localStorage");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error clearing localStorage during logout (may be prerendering)");
            }
            
            _logger.LogInformation("Logout: Logout process completed");
        }
    }

    public async Task InitializeAsync()
    {
        try
        {
            AccessToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing auth service");
        }
    }

    public UserInfo? GetUserInfo()
    {
        if (string.IsNullOrEmpty(AccessToken))
            return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(AccessToken);
            
            var username = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var displayName = jsonToken.Claims.FirstOrDefault(c => c.Type == "display_name")?.Value;

            if (string.IsNullOrEmpty(username))
                return null;

            return new UserInfo
            {
                Username = username,
                Email = email ?? string.Empty,
                DisplayName = displayName ?? username
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decoding JWT token");
            return null;
        }
    }

    /// <inheritdoc />
    public DateTime? GetTokenExpiration()
    {
        if (string.IsNullOrEmpty(AccessToken))
            return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(AccessToken))
                return null;

            var jsonToken = handler.ReadJwtToken(AccessToken);
            return jsonToken.ValidTo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading token expiration");
            return null;
        }
    }

    /// <inheritdoc />
    public bool IsTokenValid()
    {
        // Token expiration disabled for simple auth - token is valid if it exists
        return !string.IsNullOrEmpty(AccessToken);
    }

    /// <inheritdoc />
    public async Task<bool> EnsureValidTokenAsync()
    {
        // Token expiration disabled for simple auth - just check if token exists
        if (IsTokenValid())
        {
            _logger.LogDebug("EnsureValidTokenAsync: Token exists and is valid");
            return true;
        }

        // Try to load from storage if no token in memory
        await LoadTokenFromStorageAsync();
        
        // Check again after loading
        if (IsTokenValid())
        {
            _logger.LogDebug("EnsureValidTokenAsync: Token loaded from storage");
            return true;
        }

        _logger.LogWarning("EnsureValidTokenAsync: No token available");
        return false;
    }
}

