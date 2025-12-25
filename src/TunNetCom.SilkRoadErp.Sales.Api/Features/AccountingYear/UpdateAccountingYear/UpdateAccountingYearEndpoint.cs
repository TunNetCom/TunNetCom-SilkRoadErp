using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.UpdateAccountingYear;

public class UpdateAccountingYearEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/accounting-years/{id:int}", HandleUpdateAccountingYearAsync)
            .WithTags(EndpointTags.AccountingYears)
            .Produces<AccountingYearResponse>()
            .ProducesValidationProblem();
    }

    public static async Task<Results<Ok<AccountingYearResponse>, ValidationProblem>> HandleUpdateAccountingYearAsync(
        IMediator mediator,
        int id,
        UpdateAccountingYearRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAccountingYearCommand(
            Id: id,
            Year: request.Year,
            IsActive: request.IsActive
        );

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}
