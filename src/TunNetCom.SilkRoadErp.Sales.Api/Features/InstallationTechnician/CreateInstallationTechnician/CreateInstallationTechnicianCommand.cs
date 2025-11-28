using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Requests;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.CreateInstallationTechnician;

public record CreateInstallationTechnicianCommand(
    string Nom,
    string? Tel,
    string? Tel2,
    string? Tel3,
    string? Email,
    string? Description,
    string? Photo) : IRequest<Result<int>>;

