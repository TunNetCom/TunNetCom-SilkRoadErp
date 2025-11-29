using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
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
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public string? AccessToken
    {
        get => _tokenStore.GetToken(_circuitIdService.GetCircuitId());
        private set
        {
            if (!string.IsNullOrEmpty(value))
            {
                _tokenStore.SetToken(_circuitIdService.GetCircuitId(), value);
            }
        }
    }
    
    // Public setter for AuthHttpClientHandler to set token directly (avoids timeout issues)
    public void SetAccessToken(string? token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _tokenStore.SetToken(_circuitIdService.GetCircuitId(), token);
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
            _logger.LogInformation("LoadTokenFromStorageAsync: Attempting to load token from localStorage");
            
            // Use a timeout to prevent indefinite blocking
            var loadValueTask = _jsRuntime.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);
            var loadTask = loadValueTask.AsTask();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2));
            var completedTask = await Task.WhenAny(loadTask, timeoutTask);
            
            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("LoadTokenFromStorageAsync: Timeout loading token from localStorage");
                AccessToken = null;
                return;
            }
            
            AccessToken = await loadTask;
            _logger.LogInformation("LoadTokenFromStorageAsync: Token loaded. HasToken: {HasToken}", !string.IsNullOrEmpty(AccessToken));
        }
        catch (JSDisconnectedException ex)
        {
            // Component was disposed or circuit disconnected, ignore
            _logger.LogWarning(ex, "LoadTokenFromStorageAsync: JSDisconnectedException - circuit disconnected");
            AccessToken = null;
        }
        catch (InvalidOperationException ex)
        {
            // JS interop not available, ignore
            _logger.LogWarning(ex, "LoadTokenFromStorageAsync: InvalidOperationException - JS interop not available");
            AccessToken = null;
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("LoadTokenFromStorageAsync: Task was cancelled");
            AccessToken = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoadTokenFromStorageAsync: Unexpected error loading token");
            AccessToken = null;
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            _logger.LogWarning("RefreshTokenAsync: Starting token refresh");
            
            var refreshToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogError("RefreshTokenAsync: No refresh token found in localStorage");
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
                _logger.LogError("RefreshTokenAsync: Token refresh failed with status {StatusCode}", response.StatusCode);
                // Don't call LogoutAsync here - let AutoLogoutService handle it
                return false;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse == null)
            {
                _logger.LogError("RefreshTokenAsync: LoginResponse is null");
                return false;
            }

            if (string.IsNullOrEmpty(loginResponse.AccessToken))
            {
                _logger.LogError("RefreshTokenAsync: AccessToken in response is null or empty");
                return false;
            }

            _logger.LogInformation("RefreshTokenAsync: Updating tokens in localStorage and memory");

            // Update tokens in localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, loginResponse.AccessToken);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, loginResponse.RefreshToken);
            
            AccessToken = loginResponse.AccessToken;
            
            _logger.LogWarning("RefreshTokenAsync: ===== TOKEN REFRESH SUCCESSFUL =====");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RefreshTokenAsync: ===== EXCEPTION during token refresh =====");
            // Don't call LogoutAsync here - let AutoLogoutService handle it
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
            // Clear token from memory FIRST
            AccessToken = null;
            _logger.LogInformation("Logout: Token cleared from memory");
            
            // Clear token from circuit store
            var circuitId = _circuitIdService.GetCircuitId();
            _tokenStore.ClearToken(circuitId);
            _logger.LogInformation("Logout: Token cleared from TokenStore for circuit {CircuitId}", circuitId);
            
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
}

