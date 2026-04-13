using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Requests;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.UpdateInstallationTechnician;

public class UpdateInstallationTechnicianEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/installation-technician/{id}", HandleUpdateInstallationTechnicianAsync)
            .WithTags("InstallationTechnician");
    }

    public async Task<Results<NoContent, ValidationProblem, NotFound>> HandleUpdateInstallationTechnicianAsync(
        int id,
        IMediator mediator,
        UpdateInstallationTechnicianRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateInstallationTechnicianCommand(
            id,
            request.Nom,
            request.Tel,
            request.Tel2,
            request.Tel3,
            request.Email,
            request.Description,
            request.Photo);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            if (result.Errors.Any(e => e.Message.Contains("not_found")))
            {
                return TypedResults.NotFound();
            }
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

