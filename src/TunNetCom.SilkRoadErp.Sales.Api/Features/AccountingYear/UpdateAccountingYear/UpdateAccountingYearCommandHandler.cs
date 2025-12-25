using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.UpdateAccountingYear;

public class UpdateAccountingYearCommandHandler(
    SalesContext _context,
    ILogger<UpdateAccountingYearCommandHandler> _logger)
    : IRequestHandler<UpdateAccountingYearCommand, Result<AccountingYearResponse>>
{
    public async Task<Result<AccountingYearResponse>> Handle(UpdateAccountingYearCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating accounting year with Id {AccountingYearId}", command.Id);

        var accountingYear = await _context.AccountingYear
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(ay => ay.Id == command.Id, cancellationToken);

        if (accountingYear == null)
        {
            return Result.Fail("accounting_year_not_found");
        }

        // Vérifier si l'exercice est utilisé (a des relations)
        var isUsed = await _context.Facture.AnyAsync(f => f.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.FactureFournisseur.AnyAsync(f => f.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.BonDeLivraison.AnyAsync(b => b.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.BonDeReception.AnyAsync(b => b.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.Avoirs.AnyAsync(a => a.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.AvoirFournisseur.AnyAsync(a => a.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.FactureAvoirFournisseur.AnyAsync(f => f.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.FactureAvoirClient.AnyAsync(f => f.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.Inventaire.AnyAsync(i => i.AccountingYearId == command.Id, cancellationToken);

        if (isUsed)
        {
            _logger.LogWarning("Cannot update accounting year {Id} because it is used", command.Id);
            return Result.Fail("accounting_year_used_cannot_modify");
        }

        // Vérifier l'unicité de l'année si elle change
        if (accountingYear.Year != command.Year)
        {
            var yearExists = await _context.AccountingYear
                .IgnoreQueryFilters()
                .AsNoTracking()
                .AnyAsync(ay => ay.Year == command.Year && ay.Id != command.Id, cancellationToken);

            if (yearExists)
            {
                return Result.Fail("accounting_year_already_exists");
            }
        }

        // Si on active cet exercice, désactiver tous les autres
        if (command.IsActive && !accountingYear.IsActive)
        {
            var allYears = await _context.AccountingYear
                .IgnoreQueryFilters()
                .Where(ay => ay.IsActive && ay.Id != command.Id)
                .ToListAsync(cancellationToken);

            foreach (var year in allYears)
            {
                year.SetInactive();
            }
        }

        accountingYear.UpdateAccountingYear(command.Year, command.IsActive);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Accounting year {AccountingYearId} updated successfully", command.Id);

        return new AccountingYearResponse
        {
            Id = accountingYear.Id,
            Year = accountingYear.Year,
            IsActive = accountingYear.IsActive
        };
    }
}
