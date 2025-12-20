using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.CreateAccountingYear;

public class CreateAccountingYearEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/accounting-years", HandleCreateAccountingYearAsync)
            .WithTags(EndpointTags.AccountingYears)
            .Produces<AccountingYearResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    public static async Task<Results<Created<AccountingYearResponse>, ValidationProblem>> HandleCreateAccountingYearAsync(
        IMediator mediator,
        CreateAccountingYearRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAccountingYearCommand(
            Year: request.Year,
            IsActive: request.IsActive
        );

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/accounting-years/{result.Value.Id}", result.Value);
    }
}
