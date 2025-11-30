using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

[Authorize]
public class InstallationTechnicianBaseInfosController : ODataController
{
    private readonly SalesContext _context;
    private readonly ILogger<InstallationTechnicianBaseInfosController> _logger;

    public InstallationTechnicianBaseInfosController(
        SalesContext context,
        ILogger<InstallationTechnicianBaseInfosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public IActionResult Get()
    {
        try
        {
            _logger.LogInformation("InstallationTechnicianBaseInfosController.Get called");

            var techniciansQuery = _context.InstallationTechnician
                .AsNoTracking()
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
                .AsQueryable();

            _logger.LogInformation("Returning query for installation technicians");
            return Ok(techniciansQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in InstallationTechnicianBaseInfosController.Get: {Message}", ex.Message);
            return StatusCode(500, new { error = "Internal server error", message = ex.Message });
        }
    }
}

