using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.BankStatement;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.BankTransactions.ExportBankTransactionsToSage;

public class ExportBankTransactionsToSageQueryHandler(
    SalesContext _context,
    SageErpExportService _exportService,
    ILogger<ExportBankTransactionsToSageQueryHandler> _logger)
    : IRequestHandler<ExportBankTransactionsToSageQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(ExportBankTransactionsToSageQuery query, CancellationToken cancellationToken)
    {
        var q = _context.BankTransaction.AsNoTracking()
            .Include(t => t.BankTransactionImport)
            .AsQueryable();

        if (query.ImportId.HasValue)
        {
            q = q.Where(t => t.BankTransactionImportId == query.ImportId.Value);
        }

        if (query.CompteBancaireId.HasValue)
        {
            q = q.Where(t => t.BankTransactionImport!.CompteBancaireId == query.CompteBancaireId.Value);
        }

        if (query.DateDebut.HasValue)
        {
            var d = query.DateDebut.Value.Date;
            q = q.Where(t => t.DateOperation >= d);
        }

        if (query.DateFin.HasValue)
        {
            var d = query.DateFin.Value.Date.AddDays(1).AddTicks(-1);
            q = q.Where(t => t.DateOperation <= d);
        }

        var transactions = await q.OrderBy(t => t.DateOperation).ThenBy(t => t.Id).ToListAsync(cancellationToken).ConfigureAwait(false);

        if (transactions.Count == 0)
        {
            _logger.LogInformation("No bank transactions to export");
            return Result.Ok(_exportService.ExportBankTransactionsToSageFormat(Array.Empty<BankTransactionSageLine>(), "BQ"));
        }

        var sageLines = BankTransactionToSageMapper.MapToSageLines(transactions);
        var fileBytes = _exportService.ExportBankTransactionsToSageFormat(sageLines, "BQ");
        _logger.LogInformation("Exported {Count} Sage lines from {TxCount} transactions", sageLines.Count, transactions.Count);
        return Result.Ok(fileBytes);
    }
}
