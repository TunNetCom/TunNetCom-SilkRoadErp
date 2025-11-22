namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.GetAllAccountingYears;

public class GetAllAccountingYearsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/accountingYear", HandleGetAllAccountingYearsAsync)
            .WithTags("AccountingYear");
    }

    public async Task<Ok<List<GetAllAccountingYearsResponse>>> HandleGetAllAccountingYearsAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllAccountingYearsQuery(), cancellationToken);

        return TypedResults.Ok(result.Value);
    }
}

