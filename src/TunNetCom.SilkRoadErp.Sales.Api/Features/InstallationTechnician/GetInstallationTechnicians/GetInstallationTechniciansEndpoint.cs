using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.GetInstallationTechnicians;

public class GetInstallationTechniciansEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/installation-technicians", HandleGetInstallationTechniciansAsync)
            .WithTags("InstallationTechnician");
    }

    public async Task<Ok<List<InstallationTechnicianResponse>>> HandleGetInstallationTechniciansAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetInstallationTechniciansQuery();
        var result = await mediator.Send(query, cancellationToken);

        return TypedResults.Ok(result.Value);
    }
}

