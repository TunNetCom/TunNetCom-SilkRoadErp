using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.InstallationTechnician.GetInstallationTechniciansBaseInfos;

public class GetInstallationTechniciansBaseInfosEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/installation-technicians/base-infos", HandleGetInstallationTechniciansBaseInfosAsync)
            .WithTags("InstallationTechnician");
    }

    public async Task<Ok<List<InstallationTechnicianBaseInfo>>> HandleGetInstallationTechniciansBaseInfosAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetInstallationTechniciansBaseInfosQuery();
        var result = await mediator.Send(query, cancellationToken);

        return TypedResults.Ok(result.Value);
    }
}

