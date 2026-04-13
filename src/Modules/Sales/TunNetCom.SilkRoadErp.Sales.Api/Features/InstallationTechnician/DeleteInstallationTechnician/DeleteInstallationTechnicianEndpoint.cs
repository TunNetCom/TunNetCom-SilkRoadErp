namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.DeleteInstallationTechnician;

public class DeleteInstallationTechnicianEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/installation-technician/{id}", HandleDeleteInstallationTechnicianAsync)
            .WithTags("InstallationTechnician");
    }

    public async Task<Results<NoContent, ValidationProblem, NotFound>> HandleDeleteInstallationTechnicianAsync(
        int id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteInstallationTechnicianCommand(id);
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

