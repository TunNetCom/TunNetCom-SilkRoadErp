using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Requests;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.UpdateInstallationTechnician;

public record UpdateInstallationTechnicianCommand(
    int Id,
    string Nom,
    string? Tel,
    string? Tel2,
    string? Tel3,
    string? Email,
    string? Description,
    string? Photo) : IRequest<Result>;

