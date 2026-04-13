using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.DeleteInstallationTechnician;

public class DeleteInstallationTechnicianCommandHandler(
    SalesContext _context,
    ILogger<DeleteInstallationTechnicianCommandHandler> _logger)
    : IRequestHandler<DeleteInstallationTechnicianCommand, Result>
{
    public async Task<Result> Handle(DeleteInstallationTechnicianCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteInstallationTechnicianCommand called with Id {Id}", command.Id);

        var technician = await _context.InstallationTechnician
            .FindAsync([command.Id], cancellationToken);

        if (technician is null)
        {
            return Result.Fail("installation_technician_not_found");
        }

        _context.InstallationTechnician.Remove(technician);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("InstallationTechnician deleted successfully with Id {Id}", command.Id);
        return Result.Ok();
    }
}

