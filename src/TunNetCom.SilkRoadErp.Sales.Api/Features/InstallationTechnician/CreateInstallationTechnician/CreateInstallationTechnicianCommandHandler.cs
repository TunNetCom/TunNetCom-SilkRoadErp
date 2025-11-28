using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.CreateInstallationTechnician;

public class CreateInstallationTechnicianCommandHandler(
    SalesContext _context,
    ILogger<CreateInstallationTechnicianCommandHandler> _logger)
    : IRequestHandler<CreateInstallationTechnicianCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateInstallationTechnicianCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateInstallationTechnicianCommand called with Nom {Nom}", command.Nom);

        // Check if technician with same name already exists
        var technicianExists = await _context.InstallationTechnician
            .AnyAsync(t => t.Nom == command.Nom, cancellationToken);
        if (technicianExists)
        {
            return Result.Fail("installation_technician_name_already_exists");
        }

        var technician = Domain.Entites.InstallationTechnician.CreateInstallationTechnician(
            command.Nom,
            command.Tel,
            command.Tel2,
            command.Tel3,
            command.Email,
            command.Description,
            command.Photo);

        _context.InstallationTechnician.Add(technician);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("InstallationTechnician created successfully with Id {Id}", technician.Id);
        return Result.Ok(technician.Id);
    }
}

