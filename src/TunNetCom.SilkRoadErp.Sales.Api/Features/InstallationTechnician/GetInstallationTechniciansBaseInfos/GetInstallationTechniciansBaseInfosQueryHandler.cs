using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.GetInstallationTechniciansBaseInfos;

public class GetInstallationTechniciansBaseInfosQueryHandler(
    SalesContext _context,
    ILogger<GetInstallationTechniciansBaseInfosQueryHandler> _logger)
    : IRequestHandler<GetInstallationTechniciansBaseInfosQuery, Result<List<InstallationTechnicianBaseInfo>>>
{
    public async Task<Result<List<InstallationTechnicianBaseInfo>>> Handle(
        GetInstallationTechniciansBaseInfosQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetInstallationTechniciansBaseInfosQuery called");

        var technicians = await _context.InstallationTechnician
            .Select(t => new InstallationTechnicianBaseInfo
            {
                Id = t.Id,
                Nom = t.Nom,
                Photo = t.Photo
            })
            .OrderBy(t => t.Nom)
            .ToListAsync(cancellationToken);

        return Result.Ok(technicians);
    }
}

