using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Requests;
using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.InstallationTechnician;

public interface IInstallationTechnicianApiClient
{
    Task<List<InstallationTechnicianResponse>> GetInstallationTechniciansAsync(CancellationToken cancellationToken = default);
    Task<List<InstallationTechnicianBaseInfo>> GetInstallationTechniciansBaseInfosAsync(CancellationToken cancellationToken = default);
    Task<InstallationTechnicianResponse?> GetInstallationTechnicianByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateInstallationTechnicianAsync(CreateInstallationTechnicianRequest request, CancellationToken cancellationToken = default);
    Task UpdateInstallationTechnicianAsync(int id, UpdateInstallationTechnicianRequest request, CancellationToken cancellationToken = default);
    Task DeleteInstallationTechnicianAsync(int id, CancellationToken cancellationToken = default);
}

