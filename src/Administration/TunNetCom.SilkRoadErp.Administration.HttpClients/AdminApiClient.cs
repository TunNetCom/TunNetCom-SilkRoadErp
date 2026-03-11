using System.Net.Http.Json;
using TunNetCom.SilkRoadErp.Administration.Contracts.BoundedContexts;
using TunNetCom.SilkRoadErp.Administration.Contracts.Plans;
using TunNetCom.SilkRoadErp.Administration.Contracts.Tenants;

namespace TunNetCom.SilkRoadErp.Administration.HttpClients;

public interface IAdminApiClient
{
    // Tenants
    Task<IReadOnlyList<TenantSummaryDto>> GetTenantsAsync(CancellationToken ct = default);
    Task<TenantDto?> ResolveTenantAsync(string identifier, CancellationToken ct = default);
    Task<string> RegisterTenantAsync(CreateTenantDto request, CancellationToken ct = default);
    Task UpdateTenantAsync(string id, UpdateTenantDto dto, CancellationToken ct = default);
    Task DeleteTenantAsync(string id, CancellationToken ct = default);
    Task BlockTenantAsync(string id, string reason, CancellationToken ct = default);
    Task UnblockTenantAsync(string id, CancellationToken ct = default);

    // Plans
    Task<IReadOnlyList<PlanDto>> GetPlansAsync(CancellationToken ct = default);
    Task<PlanDto?> CreatePlanAsync(CreatePlanDto dto, CancellationToken ct = default);
    Task UpdatePlanAsync(int id, UpdatePlanDto dto, CancellationToken ct = default);
    Task DeletePlanAsync(int id, CancellationToken ct = default);

    // Bounded Contexts
    Task<IReadOnlyList<BoundedContextSummaryDto>> GetBoundedContextsAsync(CancellationToken ct = default);
    Task<BoundedContextDetailDto?> GetBoundedContextAsync(int id, CancellationToken ct = default);
    Task CreateBoundedContextAsync(CreateBoundedContextDto dto, CancellationToken ct = default);
    Task UpdateBoundedContextAsync(int id, UpdateBoundedContextDto dto, CancellationToken ct = default);
    Task DeleteBoundedContextAsync(int id, CancellationToken ct = default);

    // Features
    Task<IReadOnlyList<FeatureSummaryDto>> GetFeaturesAsync(int boundedContextId, CancellationToken ct = default);
    Task CreateFeatureAsync(int boundedContextId, CreateFeatureDto dto, CancellationToken ct = default);
    Task UpdateFeatureAsync(int boundedContextId, int id, UpdateFeatureDto dto, CancellationToken ct = default);
    Task DeleteFeatureAsync(int boundedContextId, int id, CancellationToken ct = default);
}

public sealed class AdminApiClient : IAdminApiClient
{
    private readonly HttpClient _http;

    public AdminApiClient(HttpClient http)
    {
        _http = http;
    }

    // ── Tenants ──

    public async Task<IReadOnlyList<TenantSummaryDto>> GetTenantsAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<TenantSummaryDto>>("/tenants", ct);
        return result ?? [];
    }

    public async Task<TenantDto?> ResolveTenantAsync(string identifier, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"/tenants/resolve/{identifier}", ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TenantDto>(ct);
    }

    public async Task<string> RegisterTenantAsync(CreateTenantDto request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/register", request, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RegisterResult>(ct);
        return result?.TenantId ?? string.Empty;
    }

    public async Task UpdateTenantAsync(string id, UpdateTenantDto dto, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"/tenants/{id}", dto, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTenantAsync(string id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"/tenants/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task BlockTenantAsync(string id, string reason, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync($"/tenants/{id}/block", new { Reason = reason }, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnblockTenantAsync(string id, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync($"/tenants/{id}/unblock", new { }, ct);
        response.EnsureSuccessStatusCode();
    }

    // ── Plans ──

    public async Task<IReadOnlyList<PlanDto>> GetPlansAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<PlanDto>>("/plans", ct);
        return result ?? [];
    }

    public async Task<PlanDto?> CreatePlanAsync(CreatePlanDto dto, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/plans", dto, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PlanDto>(ct);
    }

    public async Task UpdatePlanAsync(int id, UpdatePlanDto dto, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"/plans/{id}", dto, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeletePlanAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"/plans/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    // ── Bounded Contexts ──

    public async Task<IReadOnlyList<BoundedContextSummaryDto>> GetBoundedContextsAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<BoundedContextSummaryDto>>("/bounded-contexts", ct);
        return result ?? [];
    }

    public async Task<BoundedContextDetailDto?> GetBoundedContextAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"/bounded-contexts/{id}", ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<BoundedContextDetailDto>(ct);
    }

    public async Task CreateBoundedContextAsync(CreateBoundedContextDto dto, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/bounded-contexts", dto, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateBoundedContextAsync(int id, UpdateBoundedContextDto dto, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"/bounded-contexts/{id}", dto, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteBoundedContextAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"/bounded-contexts/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    // ── Features ──

    public async Task<IReadOnlyList<FeatureSummaryDto>> GetFeaturesAsync(int boundedContextId, CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<FeatureSummaryDto>>($"/bounded-contexts/{boundedContextId}/features", ct);
        return result ?? [];
    }

    public async Task CreateFeatureAsync(int boundedContextId, CreateFeatureDto dto, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync($"/bounded-contexts/{boundedContextId}/features", dto, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateFeatureAsync(int boundedContextId, int id, UpdateFeatureDto dto, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"/bounded-contexts/{boundedContextId}/features/{id}", dto, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteFeatureAsync(int boundedContextId, int id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"/bounded-contexts/{boundedContextId}/features/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    private record RegisterResult(string TenantId);
}
