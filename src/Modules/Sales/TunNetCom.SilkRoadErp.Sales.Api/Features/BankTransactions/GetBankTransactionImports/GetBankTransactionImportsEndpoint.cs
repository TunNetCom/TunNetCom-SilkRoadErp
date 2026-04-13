using TunNetCom.SilkRoadErp.Sales.Contracts.BankTransaction;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.BankTransactions.GetBankTransactionImports;

public class GetBankTransactionImportsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/bank-transactions/imports", HandleGetAsync)
            .WithTags("BankTransactions");
    }

    public async Task<Ok<List<BankTransactionImportResponse>>> HandleGetAsync(
        IMediator mediator,
        [FromQuery] int? compteBancaireId,
        CancellationToken cancellationToken)
    {
        var query = new GetBankTransactionImportsQuery(compteBancaireId);
        var result = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(result.Value);
    }
}
