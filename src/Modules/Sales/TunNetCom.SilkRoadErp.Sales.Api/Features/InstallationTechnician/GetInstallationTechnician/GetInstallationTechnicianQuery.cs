using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.GetInstallationTechnician;

public record GetInstallationTechnicianQuery(int Id) : IRequest<Result<InstallationTechnicianResponse>>;

