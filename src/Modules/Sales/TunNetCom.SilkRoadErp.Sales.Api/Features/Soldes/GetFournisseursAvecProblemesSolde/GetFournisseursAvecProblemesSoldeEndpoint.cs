using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetFournisseursAvecProblemesSolde;

public class GetFournisseursAvecProblemesSoldeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/soldes/fournisseurs-avec-problemes", HandleGetFournisseursAvecProblemesSoldeAsync)
            .WithTags(EndpointTags.Soldes)
            .Produces<PagedList<FournisseurSoldeProblemeResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Retrieves a paginated list of fournisseurs with non-zero balance.")
            .WithOpenApi();
    }

    public static async Task<Ok<PagedList<FournisseurSoldeProblemeResponse>>> HandleGetFournisseursAvecProblemesSoldeAsync(
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? accountingYearId = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("PageNumber and PageSize must be greater than zero.");
        }

        var query = new GetFournisseursAvecProblemesSoldeQuery(pageNumber, pageSize, accountingYearId);
        var result = await mediator.Send(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
