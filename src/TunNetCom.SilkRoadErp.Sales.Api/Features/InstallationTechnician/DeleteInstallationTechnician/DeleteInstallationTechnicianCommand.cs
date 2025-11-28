namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.DeleteInstallationTechnician;

public record DeleteInstallationTechnicianCommand(int Id) : IRequest<Result>;

