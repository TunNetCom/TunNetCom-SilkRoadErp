using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.GetAccountingYearById;

public class GetAccountingYearByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/accounting-years/{id:int}", HandleGetAccountingYearByIdAsync)
            .WithTags(EndpointTags.AccountingYears)
            .Produces<AccountingYearResponse>()
            .ProducesValidationProblem();
    }

    public static async Task<Results<Ok<AccountingYearResponse>, ValidationProblem>> HandleGetAccountingYearByIdAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetAccountingYearByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}
