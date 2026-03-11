namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.SetActiveAccountingYear;

public class SetActiveAccountingYearEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/accountingYear/active", HandleSetActiveAccountingYearAsync)
            .WithTags("AccountingYear");
    }

    public async Task<Results<Ok, ValidationProblem>> HandleSetActiveAccountingYearAsync(
        IMediator mediator,
        SetActiveAccountingYearRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SetActiveAccountingYearCommand(request.AccountingYearId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Ok();
    }
}

public record SetActiveAccountingYearRequest(int AccountingYearId);

