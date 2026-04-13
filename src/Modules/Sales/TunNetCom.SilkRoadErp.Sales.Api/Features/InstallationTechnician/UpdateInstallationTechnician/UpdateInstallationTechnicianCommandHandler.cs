using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.UpdateInstallationTechnician;

public class UpdateInstallationTechnicianCommandHandler(
    SalesContext _context,
    ILogger<UpdateInstallationTechnicianCommandHandler> _logger)
    : IRequestHandler<UpdateInstallationTechnicianCommand, Result>
{
    public async Task<Result> Handle(UpdateInstallationTechnicianCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateInstallationTechnicianCommand called with Id {Id}", command.Id);

        var technician = await _context.InstallationTechnician
            .FindAsync([command.Id], cancellationToken);

        if (technician is null)
        {
            return Result.Fail("installation_technician_not_found");
        }

        // Check if another technician with the same name exists
        var nameExists = await _context.InstallationTechnician
            .AnyAsync(t => t.Nom == command.Nom && t.Id != command.Id, cancellationToken);
        if (nameExists)
        {
            return Result.Fail("installation_technician_name_already_exists");
        }

        technician.UpdateInstallationTechnician(
            command.Nom,
            command.Tel,
            command.Tel2,
            command.Tel3,
            command.Email,
            command.Description,
            command.Photo);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("InstallationTechnician updated successfully with Id {Id}", technician.Id);
        return Result.Ok();
    }
}

