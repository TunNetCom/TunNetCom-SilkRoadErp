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
        ILogger<UpdateAccountingYearEndpoint> logger,
        int id,
        UpdateAccountingYearRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("UpdateAccountingYear endpoint - Request received: Id={Id}, Year={Year}, IsActive={IsActive}, VatAmount={VatAmount}, SeuilRetenueSource={SeuilRetenueSource}, Timbre={Timbre}, PourcentageFodec={PourcentageFodec}, VatRate0={VatRate0}, VatRate7={VatRate7}, VatRate13={VatRate13}, VatRate19={VatRate19}, PourcentageRetenu={PourcentageRetenu}",
            id, request.Year, request.IsActive, request.VatAmount, request.SeuilRetenueSource, request.Timbre, request.PourcentageFodec, request.VatRate0, request.VatRate7, request.VatRate13, request.VatRate19, request.PourcentageRetenu);

        var command = new UpdateAccountingYearCommand(
            Id: id,
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
            logger.LogError("UpdateAccountingYear failed for Id={Id}: {Errors}", id, string.Join(", ", result.Errors.Select(e => $"{e.Message} (Reason: {e.Reasons})")));
            foreach (var error in result.Errors)
            {
                logger.LogError("Error detail: {Message}, Reasons: {Reasons}", error.Message, string.Join(", ", error.Reasons.Select(r => r.Message)));
            }
            return result.ToValidationProblem();
        }

        logger.LogInformation("UpdateAccountingYear endpoint - Success: Response VatAmount={VatAmount}, SeuilRetenueSource={SeuilRetenueSource}",
            result.Value.VatAmount, result.Value.SeuilRetenueSource);
        return TypedResults.Ok(result.Value);
    }
}
