using System.Net.Http.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Users;

public class UsersClient : IUsersClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersClient> _logger;

    public UsersClient(HttpClient httpClient, ILogger<UsersClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedList<UserResponse>> GetUsersAsync(int pageNumber, int pageSize, string? searchKeyword = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryString = $"?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                queryString += $"&searchKeyword={Uri.EscapeDataString(searchKeyword)}";
            }

            var response = await _httpClient.GetAsync($"/users{queryString}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PagedList<UserResponse>>(cancellationToken: cancellationToken)
                ?? new PagedList<UserResponse>(new List<UserResponse>(), 0, 0, 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users");
            throw;
        }
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/users/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user with ID {UserId}", id);
            throw;
        }
    }

    public async Task<int> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/users", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>(cancellationToken: cancellationToken);
            return result?["id"] ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            throw;
        }
    }

    public async Task UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/users/{id}", request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", id);
            throw;
        }
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/users/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            throw;
        }
    }

    public async Task ChangePasswordAsync(int id, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/users/{id}/change-password", request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user with ID {UserId}", id);
            throw;
        }
    }

    public async Task<List<RoleResponse>> GetAvailableRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/users/roles", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<RoleResponse>>(cancellationToken: cancellationToken)
                ?? new List<RoleResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching available roles");
            throw;
        }
    }
}


