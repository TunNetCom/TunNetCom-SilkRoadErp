namespace TunNetCom.SilkRoadErp.Sales.Api.Features.BankTransactions.ExportBankTransactionsToSage;

public class ExportBankTransactionsToSageEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/bank-transactions/export/sage", HandleExportAsync)
            .WithTags("BankTransactions")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportAsync(
        IMediator mediator,
        [FromQuery] int? importId,
        [FromQuery] int? compteBancaireId,
        [FromQuery] DateTime? dateDebut,
        [FromQuery] DateTime? dateFin,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportBankTransactionsToSageQuery(importId, compteBancaireId, dateDebut, dateFin);
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (result.IsFailed)
        {
            return TypedResults.StatusCode(500);
        }

        var filename = $"Releve_Bancaire_Sage_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        return TypedResults.File(
            result.Value,
            contentType: "text/plain; charset=windows-1252",
            fileDownloadName: filename);
    }
}
