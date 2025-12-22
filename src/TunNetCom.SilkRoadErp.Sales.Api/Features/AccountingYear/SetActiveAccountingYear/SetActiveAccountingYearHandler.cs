namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.SetActiveAccountingYear;

public class SetActiveAccountingYearHandler(
    SalesContext _context,
    IActiveAccountingYearService _activeAccountingYearService,
    ILogger<SetActiveAccountingYearHandler> _logger)
    : IRequestHandler<SetActiveAccountingYearCommand, Result>
{
    public async Task<Result> Handle(SetActiveAccountingYearCommand command, CancellationToken cancellationToken)
    {
        var accountingYear = await _context.AccountingYear
            .IgnoreQueryFilters() // Ignorer les filtres globaux pour accéder à tous les exercices
            .FirstOrDefaultAsync(ay => ay.Id == command.AccountingYearId, cancellationToken);

        if (accountingYear == null)
        {
            _logger.LogWarning("Accounting year with Id {Id} not found", command.AccountingYearId);
            return Result.Fail("accounting_year_not_found");
        }

        // Deactivate all accounting years
        var allYears = await _context.AccountingYear
            .IgnoreQueryFilters() // Ignorer les filtres globaux pour accéder à tous les exercices
            .ToListAsync(cancellationToken);
        foreach (var year in allYears)
        {
            if (year.IsActive)
            {
                year.UpdateAccountingYear(year.Year, false);
            }
        }

        // Activate the selected year
        accountingYear.UpdateAccountingYear(accountingYear.Year, true);

        _ = await _context.SaveChangesAsync(cancellationToken);

        // Invalider le cache pour forcer le rechargement du nouvel exercice actif
        _activeAccountingYearService.InvalidateCache();

        // Mettre à jour l'AsyncLocal dans SalesContext pour que le global filter fonctionne immédiatement
        Domain.Entites.SalesContext.SetActiveAccountingYearId(accountingYear.Id);

        _logger.LogInformation("Accounting year {Year} (Id: {Id}) set as active and global filter updated", accountingYear.Year, accountingYear.Id);

        return Result.Ok();
    }
}

