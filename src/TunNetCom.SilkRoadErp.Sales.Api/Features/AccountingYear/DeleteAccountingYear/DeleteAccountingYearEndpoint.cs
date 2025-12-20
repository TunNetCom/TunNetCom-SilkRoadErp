using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.DeleteAccountingYear;

public class DeleteAccountingYearEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/accounting-years/{id:int}", HandleDeleteAccountingYearAsync)
            .WithTags(EndpointTags.AccountingYears)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();
    }

    public static async Task<Results<NoContent, ValidationProblem>> HandleDeleteAccountingYearAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAccountingYearCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}
