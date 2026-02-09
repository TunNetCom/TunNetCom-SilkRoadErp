using TunNetCom.SilkRoadErp.Sales.Contracts.BankTransaction;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.BankTransactions.GetBankTransactionImports;

public class GetBankTransactionImportsQueryHandler(
    SalesContext _context,
    ILogger<GetBankTransactionImportsQueryHandler> _logger)
    : IRequestHandler<GetBankTransactionImportsQuery, Result<List<BankTransactionImportResponse>>>
{
    public async Task<Result<List<BankTransactionImportResponse>>> Handle(GetBankTransactionImportsQuery query, CancellationToken cancellationToken)
    {
        var q = _context.BankTransactionImport.AsNoTracking()
            .Include(i => i.CompteBancaire)
            .ThenInclude(c => c!.Banque)
            .AsQueryable();

        if (query.CompteBancaireId.HasValue)
        {
            q = q.Where(i => i.CompteBancaireId == query.CompteBancaireId.Value);
        }

        var imports = await q.OrderByDescending(i => i.ImportedAt).ToListAsync(cancellationToken);
        var ids = imports.Select(i => i.Id).ToList();
        var counts = await _context.BankTransaction
            .Where(t => ids.Contains(t.BankTransactionImportId))
            .GroupBy(t => t.BankTransactionImportId)
            .Select(g => new { ImportId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ImportId, x => x.Count, cancellationToken);

        var list = imports.Select(i => new BankTransactionImportResponse
        {
            Id = i.Id,
            CompteBancaireId = i.CompteBancaireId,
            CompteBancaireLibelle = i.CompteBancaire != null && i.CompteBancaire.Banque != null
                ? $"{i.CompteBancaire.Banque.Nom} - {i.CompteBancaire.NumeroCompte}"
                : null,
            FileName = i.FileName,
            ImportedAt = i.ImportedAt,
            RowCount = counts.GetValueOrDefault(i.Id, 0)
        }).ToList();

        return Result.Ok(list);
    }
}
