namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public interface INumberGeneratorService
{
    Task<int> GenerateFactureNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateFactureFournisseurNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateBonDeLivraisonNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateBonDeReceptionNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
}

public class NumberGeneratorService : INumberGeneratorService
{
    private readonly SalesContext _context;
    private readonly ILogger<NumberGeneratorService> _logger;

    public NumberGeneratorService(SalesContext context, ILogger<NumberGeneratorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> GenerateFactureNumberAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        var accountingYear = await _context.AccountingYear.FindAsync(new object[] { accountingYearId }, cancellationToken);
        if (accountingYear == null)
        {
            throw new InvalidOperationException($"Accounting year with Id {accountingYearId} not found");
        }

        var year = accountingYear.Year;
        var lastNum = await _context.Facture
            .Where(f => f.AccountingYearId == accountingYearId)
            .OrderByDescending(f => f.Num)
            .Select(f => f.Num)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence; // Format: YYYY00000

        _logger.LogInformation("Generated Facture number: {Num} for accounting year {Year}", newNum, year);
        return newNum;
    }

    public async Task<int> GenerateFactureFournisseurNumberAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        var accountingYear = await _context.AccountingYear.FindAsync(new object[] { accountingYearId }, cancellationToken);
        if (accountingYear == null)
        {
            throw new InvalidOperationException($"Accounting year with Id {accountingYearId} not found");
        }

        var year = accountingYear.Year;
        var lastNum = await _context.FactureFournisseur
            .Where(f => f.AccountingYearId == accountingYearId)
            .OrderByDescending(f => f.Num)
            .Select(f => f.Num)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence;

        _logger.LogInformation("Generated FactureFournisseur number: {Num} for accounting year {Year}", newNum, year);
        return newNum;
    }

    public async Task<int> GenerateBonDeLivraisonNumberAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        var accountingYear = await _context.AccountingYear.FindAsync(new object[] { accountingYearId }, cancellationToken);
        if (accountingYear == null)
        {
            throw new InvalidOperationException($"Accounting year with Id {accountingYearId} not found");
        }

        var year = accountingYear.Year;
        var lastNum = await _context.BonDeLivraison
            .Where(b => b.AccountingYearId == accountingYearId)
            .OrderByDescending(b => b.Num)
            .Select(b => b.Num)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence;

        _logger.LogInformation("Generated BonDeLivraison number: {Num} for accounting year {Year}", newNum, year);
        return newNum;
    }

    public async Task<int> GenerateBonDeReceptionNumberAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        var accountingYear = await _context.AccountingYear.FindAsync(new object[] { accountingYearId }, cancellationToken);
        if (accountingYear == null)
        {
            throw new InvalidOperationException($"Accounting year with Id {accountingYearId} not found");
        }

        var year = accountingYear.Year;
        var lastNum = await _context.BonDeReception
            .Where(b => b.AccountingYearId == accountingYearId)
            .OrderByDescending(b => b.Num)
            .Select(b => b.Num)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence;

        _logger.LogInformation("Generated BonDeReception number: {Num} for accounting year {Year}", newNum, year);
        return newNum;
    }

    private static int GetNextSequence(int lastNum, int year)
    {
        if (lastNum == 0)
        {
            return 1; // Start with 1
        }

        // Extract sequence from lastNum (last 5 digits)
        var lastYear = lastNum / 100000;
        if (lastYear == year)
        {
            // Same year, increment sequence
            var sequence = lastNum % 100000;
            return sequence + 1;
        }
        else
        {
            // Different year, start from 1
            return 1;
        }
    }
}

