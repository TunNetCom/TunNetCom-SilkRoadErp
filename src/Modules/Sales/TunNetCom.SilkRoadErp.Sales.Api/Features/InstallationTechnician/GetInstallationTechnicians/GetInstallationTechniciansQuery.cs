using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.GetInstallationTechnicians;

public record GetInstallationTechniciansQuery : IRequest<Result<List<InstallationTechnicianResponse>>>;

