using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.BankStatement;
using TunNetCom.SilkRoadErp.Sales.Contracts.BankTransaction;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.BankTransactions.ImportBankTransactions;

public class ImportBankTransactionsCommandHandler(
    SalesContext _context,
    IBankStatementFileParser _parser,
    ILogger<ImportBankTransactionsCommandHandler> _logger)
    : IRequestHandler<ImportBankTransactionsCommand, Result<ImportBankTransactionsResponse>>
{
    public async Task<Result<ImportBankTransactionsResponse>> Handle(ImportBankTransactionsCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ImportBankTransactions for CompteBancaireId {CompteBancaireId}, File {FileName}", command.CompteBancaireId, command.FileName);

        var compteExists = await _context.CompteBancaire.AnyAsync(c => c.Id == command.CompteBancaireId, cancellationToken);
        if (!compteExists)
        {
            return Result.Fail("compte_bancaire_not_found");
        }

        IReadOnlyList<BankStatementRowDto> rows;
        var errors = new List<string>();
        try
        {
            rows = await _parser.ParseAsync(command.FileStream, command.FileName, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Parse failed for {FileName}", command.FileName);
            return Result.Fail("file_parse_error");
        }

        if (rows.Count == 0)
        {
            return Result.Ok(new ImportBankTransactionsResponse
            {
                ImportId = 0,
                RowsImported = 0,
                FileName = command.FileName,
                Errors = new[] { "Aucune ligne de transaction trouvée dans le fichier." }
            });
        }

        var importEntity = Domain.Entites.BankTransactionImport.CreateBankTransactionImport(command.CompteBancaireId, command.FileName);
        _context.BankTransactionImport.Add(importEntity);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        foreach (var row in rows)
        {
            var tx = Domain.Entites.BankTransaction.CreateBankTransaction(
                importEntity.Id,
                row.DateOperation,
                row.DateValeur,
                row.Operation,
                row.Reference,
                row.Debit,
                row.Credit);
            _context.BankTransaction.Add(tx);
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Imported {Count} rows for import Id {ImportId}", rows.Count, importEntity.Id);

        return Result.Ok(new ImportBankTransactionsResponse
        {
            ImportId = importEntity.Id,
            RowsImported = rows.Count,
            FileName = command.FileName,
            Errors = errors
        });
    }
}
