using TunNetCom.SilkRoadErp.Sales.Contracts.BankTransaction;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.BankTransactions.ImportBankTransactions;

public class ImportBankTransactionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/bank-transactions/import", HandleImportAsync)
            .WithTags("BankTransactions")
            .DisableAntiforgery()
            .Produces<ImportBankTransactionsResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }

    public async Task<Results<Ok<ImportBankTransactionsResponse>, ValidationProblem, NotFound>> HandleImportAsync(
        IMediator mediator,
        IFormFile? file,
        [FromQuery] int compteBancaireId,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["file"] = new[] { "file_is_required" }
            });
        }

        if (compteBancaireId <= 0)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["compteBancaireId"] = new[] { "compte_bancaire_is_required" }
            });
        }

        await using var stream = file.OpenReadStream();
        var command = new ImportBankTransactionsCommand(compteBancaireId, stream, file.FileName ?? "upload");
        var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);

        if (result.IsFailed)
        {
            if (result.Errors.Any(e => e.Message == "compte_bancaire_not_found"))
            {
                return TypedResults.NotFound();
            }
            return result.ToValidationProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}
