using System.Net.Http.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Requests;
using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.InstallationTechnician;

public class InstallationTechnicianApiClient : IInstallationTechnicianApiClient
{
    private readonly HttpClient _httpClient;

    public InstallationTechnicianApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<InstallationTechnicianResponse>> GetInstallationTechniciansAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<List<InstallationTechnicianResponse>>(
            "/installation-technicians",
            cancellationToken);
        return response ?? new List<InstallationTechnicianResponse>();
    }

    public async Task<List<InstallationTechnicianBaseInfo>> GetInstallationTechniciansBaseInfosAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<List<InstallationTechnicianBaseInfo>>(
            "/installation-technicians/base-infos",
            cancellationToken);
        return response ?? new List<InstallationTechnicianBaseInfo>();
    }

    public async Task<InstallationTechnicianResponse?> GetInstallationTechnicianByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<InstallationTechnicianResponse>(
            $"/installation-technician/{id}",
            cancellationToken);
    }

    public async Task<int> CreateInstallationTechnicianAsync(CreateInstallationTechnicianRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/installation-technician",
            request,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var locationHeader = response.Headers.Location?.ToString();
        if (locationHeader != null)
        {
            var id = int.Parse(locationHeader.Split('/').Last());
            return id;
        }
        return 0;
    }

    public async Task UpdateInstallationTechnicianAsync(int id, UpdateInstallationTechnicianRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"/installation-technician/{id}",
            request,
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteInstallationTechnicianAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"/installation-technician/{id}",
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

