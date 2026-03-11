using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.GetInstallationTechnician;

public class GetInstallationTechnicianQueryHandler(
    SalesContext _context,
    ILogger<GetInstallationTechnicianQueryHandler> _logger)
    : IRequestHandler<GetInstallationTechnicianQuery, Result<InstallationTechnicianResponse>>
{
    public async Task<Result<InstallationTechnicianResponse>> Handle(
        GetInstallationTechnicianQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetInstallationTechnicianQuery called with Id {Id}", query.Id);

        var technician = await _context.InstallationTechnician
            .Where(t => t.Id == query.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (technician is null)
        {
            return Result.Fail("installation_technician_not_found");
        }

        return Result.Ok(technician);
    }
}

