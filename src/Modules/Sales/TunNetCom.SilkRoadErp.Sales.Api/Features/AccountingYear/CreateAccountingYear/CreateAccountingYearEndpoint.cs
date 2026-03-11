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
            IsActive: request.IsActive,
            Timbre: request.Timbre,
            PourcentageFodec: request.PourcentageFodec,
            VatRate0: request.VatRate0,
            VatRate7: request.VatRate7,
            VatRate13: request.VatRate13,
            VatRate19: request.VatRate19,
            PourcentageRetenu: request.PourcentageRetenu,
            VatAmount: request.VatAmount,
            SeuilRetenueSource: request.SeuilRetenueSource
        );

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/accounting-years/{result.Value.Id}", result.Value);
    }
}
