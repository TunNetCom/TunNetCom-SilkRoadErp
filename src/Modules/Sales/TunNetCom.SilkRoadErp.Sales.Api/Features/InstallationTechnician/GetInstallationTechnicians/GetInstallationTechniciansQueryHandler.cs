using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.GetInstallationTechnicians;

public class GetInstallationTechniciansQueryHandler(
    SalesContext _context,
    ILogger<GetInstallationTechniciansQueryHandler> _logger)
    : IRequestHandler<GetInstallationTechniciansQuery, Result<List<InstallationTechnicianResponse>>>
{
    public async Task<Result<List<InstallationTechnicianResponse>>> Handle(
        GetInstallationTechniciansQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetInstallationTechniciansQuery called");

        var technicians = await _context.InstallationTechnician
            .Select(t => new InstallationTechnicianResponse
            {
                Id = t.Id,
                Nom = t.Nom,
                Tel = t.Tel,
                Tel2 = t.Tel2,
                Tel3 = t.Tel3,
                Email = t.Email,
                Description = t.Description,
                Photo = t.Photo
            })
            .OrderBy(t => t.Nom)
            .ToListAsync(cancellationToken);

        return Result.Ok(technicians);
    }
}

