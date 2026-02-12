using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public interface INumberGeneratorService
{
    /// <summary>
    /// Génère la prochaine référence certificat TEJ pour le mois donné. Format : {Mois}{XXX} (ex. mars → 3001, 3002).
    /// La séquence est réinitialisée chaque mois.
    /// </summary>
    Task<string> GetNextTejCertificatRefAsync(int year, int month, CancellationToken cancellationToken = default);

    Task<int> GenerateFactureNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateFactureFournisseurNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateBonDeLivraisonNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateBonDeReceptionNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateAvoirNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateAvoirFournisseurNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateFactureAvoirFournisseurNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateAvoirFinancierFournisseurNumAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateRetourMarchandiseFournisseurNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<int> GenerateFactureDepenseNumberAsync(int accountingYearId, CancellationToken cancellationToken = default);
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

    public async Task<string> GetNextTejCertificatRefAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        if (month < 1 || month > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Le mois doit être entre 1 et 12.");

        const int maxRetries = 3;
        for (var retry = 0; retry < maxRetries; retry++)
        {
            var row = await _context.TejCertificatSequence
                .AsTracking()
                .FirstOrDefaultAsync(s => s.Annee == year && s.Mois == month, cancellationToken);

            if (row == null)
            {
                row = new TejCertificatSequence { Annee = year, Mois = month, DerniereSequence = 0 };
                _context.TejCertificatSequence.Add(row);
            }

            row.DerniereSequence++;
            var sequence = row.DerniereSequence;
            var refCertif = month.ToString() + sequence.ToString("D3"); // ex. 3 + 001 = 3001, 12 + 001 = 12001

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Generated TEJ certificat ref: {Ref} for {Year}/{Month}", refCertif, year, month);
                return refCertif;
            }
            catch (DbUpdateConcurrencyException) when (retry < maxRetries - 1)
            {
                _context.Entry(row).State = EntityState.Detached;
            }
        }

        throw new InvalidOperationException("Impossible de générer la référence certificat TEJ après plusieurs tentatives.");
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
            .IgnoreQueryFilters() // Désactiver le filtre global pour contrôler explicitement le filtre
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
            .IgnoreQueryFilters() // Désactiver le filtre global pour contrôler explicitement le filtre
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
            .IgnoreQueryFilters() // Désactiver le filtre global pour contrôler explicitement le filtre
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
            .IgnoreQueryFilters() // Désactiver le filtre global pour contrôler explicitement le filtre
            .Where(b => b.AccountingYearId == accountingYearId)
            .OrderByDescending(b => b.Num)
            .Select(b => b.Num)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence;

        _logger.LogInformation("Generated BonDeReception number: {Num} for accounting year {Year}", newNum, year);
        return newNum;
    }

    public async Task<int> GenerateAvoirNumberAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        var accountingYear = await _context.AccountingYear.FindAsync(new object[] { accountingYearId }, cancellationToken);
        if (accountingYear == null)
        {
            throw new InvalidOperationException($"Accounting year with Id {accountingYearId} not found");
        }

        var year = accountingYear.Year;
        var lastNum = await _context.Avoirs
            .IgnoreQueryFilters() // Désactiver le filtre global pour contrôler explicitement le filtre
            .Where(a => a.AccountingYearId == accountingYearId)
            .OrderByDescending(a => a.Num)
            .Select(a => a.Num)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence;

        _logger.LogInformation("Generated Avoir number: {Num} for accounting year {Year}", newNum, year);
        return newNum;
    }

    public async Task<int> GenerateAvoirFournisseurNumberAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        var accountingYear = await _context.AccountingYear.FindAsync(new object[] { accountingYearId }, cancellationToken);
        if (accountingYear == null)
        {
            throw new InvalidOperationException($"Accounting year with Id {accountingYearId} not found");
        }

        var year = accountingYear.Year;
        var lastNum = await _context.AvoirFournisseur
            .IgnoreQueryFilters() // Désactiver le filtre global pour contrôler explicitement le filtre
            .Where(a => a.AccountingYearId == accountingYearId)
            .OrderByDescending(a => a.NumAvoirChezFournisseur)
            .Select(a => a.NumAvoirChezFournisseur)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence;

        _logger.LogInformation("Generated AvoirFournisseur number: {Num} for accounting year {Year}", newNum, year);
        return newNum;
    }

    public async Task<int> GenerateFactureAvoirFournisseurNumberAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        var accountingYear = await _context.AccountingYear.FindAsync(new object[] { accountingYearId }, cancellationToken);
        if (accountingYear == null)
        {
            throw new InvalidOperationException($"Accounting year with Id {accountingYearId} not found");
        }

        var year = accountingYear.Year;
        var lastNum = await _context.FactureAvoirFournisseur
            .IgnoreQueryFilters() // Désactiver le filtre global pour contrôler explicitement le filtre
            .Where(f => f.AccountingYearId == accountingYearId)
            .OrderByDescending(f => f.NumFactureAvoirFourSurPage)
            .Select(f => f.NumFactureAvoirFourSurPage)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence;

        _logger.LogInformation("Generated FactureAvoirFournisseur number: {Num} for accounting year {Year}", newNum, year);
        return newNum;
    }

    public async Task<int> GenerateAvoirFinancierFournisseurNumAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        var accountingYear = await _context.AccountingYear.FindAsync(new object[] { accountingYearId }, cancellationToken);
        if (accountingYear == null)
        {
            throw new InvalidOperationException($"Accounting year with Id {accountingYearId} not found");
        }

        var year = accountingYear.Year;
        var lastNum = await _context.AvoirFinancierFournisseurs
            .IgnoreQueryFilters()
            .MaxAsync(a => (int?)a.Num, cancellationToken) ?? 0;

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence;

        _logger.LogInformation("Generated AvoirFinancierFournisseur number: {Num} for accounting year {Year}", newNum, year);
        return newNum;
    }

    public async Task<int> GenerateRetourMarchandiseFournisseurNumberAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        var accountingYear = await _context.AccountingYear.FindAsync(new object[] { accountingYearId }, cancellationToken);
        if (accountingYear == null)
        {
            throw new InvalidOperationException($"Accounting year with Id {accountingYearId} not found");
        }

        var year = accountingYear.Year;
        var lastNum = await _context.RetourMarchandiseFournisseur
            .IgnoreQueryFilters() // Désactiver le filtre global pour contrôler explicitement le filtre
            .Where(r => r.AccountingYearId == accountingYearId)
            .OrderByDescending(r => r.Num)
            .Select(r => r.Num)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence;

        _logger.LogInformation("Generated RetourMarchandiseFournisseur number: {Num} for accounting year {Year}", newNum, year);
        return newNum;
    }

    public async Task<int> GenerateFactureDepenseNumberAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        var accountingYear = await _context.AccountingYear.FindAsync(new object[] { accountingYearId }, cancellationToken);
        if (accountingYear == null)
        {
            throw new InvalidOperationException($"Accounting year with Id {accountingYearId} not found");
        }

        var year = accountingYear.Year;
        var lastNum = await _context.FactureDepense
            .IgnoreQueryFilters()
            .Where(f => f.AccountingYearId == accountingYearId)
            .OrderByDescending(f => f.Num)
            .Select(f => f.Num)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = GetNextSequence(lastNum, year);
        var newNum = year * 100000 + nextSequence;

        _logger.LogInformation("Generated FactureDepense number: {Num} for accounting year {Year}", newNum, year);
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

