using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.GetInstallationTechniciansBaseInfos;

public record GetInstallationTechniciansBaseInfosQuery : IRequest<Result<List<InstallationTechnicianBaseInfo>>>;

