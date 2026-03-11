namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.SetActiveAccountingYear;

public class SetActiveAccountingYearHandler(
    SalesContext _context,
    IActiveAccountingYearService _activeAccountingYearService,
    IAccountingYearFinancialParametersService _financialParametersService,
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

        _logger.LogInformation("SetActiveAccountingYearHandler: Saved changes, old active year was deactivated, new active year is {Year} (Id: {Id})", 
            accountingYear.Year, accountingYear.Id);

        // Mettre à jour directement le cache avec le nouvel ID pour que les requêtes suivantes utilisent la bonne valeur
        _activeAccountingYearService.SetActiveAccountingYearId(accountingYear.Id);
        
        // Invalider le cache des paramètres financiers pour forcer le rechargement avec la nouvelle année
        _financialParametersService.InvalidateCache();

        // Mettre à jour l'AsyncLocal dans SalesContext pour que le global filter fonctionne immédiatement pour cette requête
        var previousAsyncLocalValue = Domain.Entites.SalesContext.GetActiveAccountingYearId();
        Domain.Entites.SalesContext.SetActiveAccountingYearId(accountingYear.Id);
        
        _logger.LogInformation("SetActiveAccountingYearHandler: Updated cache and AsyncLocal from {OldAsyncLocal} to {NewAsyncLocal} for accounting year {Year} (Id: {Id})", 
            previousAsyncLocalValue, accountingYear.Id, accountingYear.Year, accountingYear.Id);

        return Result.Ok();
    }
}

