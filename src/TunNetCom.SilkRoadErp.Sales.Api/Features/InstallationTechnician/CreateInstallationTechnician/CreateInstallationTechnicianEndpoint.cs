using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Requests;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.CreateInstallationTechnician;

public class CreateInstallationTechnicianEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/installation-technician", HandleCreateInstallationTechnicianAsync)
            .WithTags("InstallationTechnician");
    }

    public async Task<Results<Created<CreateInstallationTechnicianRequest>, ValidationProblem>> HandleCreateInstallationTechnicianAsync(
        IMediator mediator,
        CreateInstallationTechnicianRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateInstallationTechnicianCommand(
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
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/installation-technician/{result.Value}", request);
    }
}

