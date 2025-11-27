using System.Net.Http.Json;
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
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;
    private readonly IJSRuntime _jsRuntime;
    private const string AccessTokenKey = "auth_access_token";
    private const string RefreshTokenKey = "auth_refresh_token";

    public AuthService(HttpClient httpClient, ILogger<AuthService> logger, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsRuntime = jsRuntime;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public string? AccessToken { get; private set; }

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

            // Store tokens in localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, loginResponse.AccessToken);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, loginResponse.RefreshToken);
            
            AccessToken = loginResponse.AccessToken;
            
            _logger.LogInformation("Login successful for user {Username}", username);
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
            AccessToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);
        }
        catch (JSDisconnectedException)
        {
            // Component was disposed or circuit disconnected, ignore
            AccessToken = null;
        }
        catch (InvalidOperationException)
        {
            // JS interop not available, ignore
            AccessToken = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading token from storage");
            AccessToken = null;
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            var request = new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/refresh", request);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Token refresh failed with status {StatusCode}", response.StatusCode);
                await LogoutAsync();
                return false;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse == null)
            {
                return false;
            }

            // Update tokens in localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, loginResponse.AccessToken);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, loginResponse.RefreshToken);
            
            AccessToken = loginResponse.AccessToken;
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            await LogoutAsync();
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var refreshToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var request = new { RefreshToken = refreshToken };
                await _httpClient.PostAsJsonAsync("/api/auth/logout", request);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
        }
        finally
        {
            // Clear tokens from localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
            AccessToken = null;
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
}

