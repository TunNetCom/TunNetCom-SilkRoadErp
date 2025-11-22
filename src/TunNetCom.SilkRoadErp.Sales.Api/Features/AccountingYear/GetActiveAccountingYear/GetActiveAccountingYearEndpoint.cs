namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.GetActiveAccountingYear;

public class GetActiveAccountingYearEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/accountingYear/active", HandleGetActiveAccountingYearAsync)
            .WithTags("AccountingYear");
    }

    public async Task<Results<Ok<GetActiveAccountingYearResponse>, NotFound>> HandleGetActiveAccountingYearAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetActiveAccountingYearQuery(), cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

