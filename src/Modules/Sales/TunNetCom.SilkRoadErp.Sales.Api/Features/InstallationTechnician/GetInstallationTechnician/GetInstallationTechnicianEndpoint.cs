using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.GetInstallationTechnician;

public class GetInstallationTechnicianEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/installation-technician/{id}", HandleGetInstallationTechnicianAsync)
            .WithTags("InstallationTechnician");
    }

    public async Task<Results<Ok<InstallationTechnicianResponse>, NotFound>> HandleGetInstallationTechnicianAsync(
        int id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetInstallationTechnicianQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

