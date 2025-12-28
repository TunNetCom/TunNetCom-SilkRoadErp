using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.UpdateAccountingYear;

public class UpdateAccountingYearCommandHandler(
    SalesContext _context,
    IAccountingYearFinancialParametersService _financialParametersService,
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

        _logger.LogInformation("Before UpdateAccountingYear - Command values: VatAmount={VatAmount}, SeuilRetenueSource={SeuilRetenueSource}, Timbre={Timbre}, PourcentageFodec={PourcentageFodec}",
            command.VatAmount, command.SeuilRetenueSource, command.Timbre, command.PourcentageFodec);
        _logger.LogInformation("Before UpdateAccountingYear - Current entity values: VatAmount={VatAmount}, SeuilRetenueSource={SeuilRetenueSource}, Timbre={Timbre}, PourcentageFodec={PourcentageFodec}",
            accountingYear.VatAmount, accountingYear.SeuilRetenueSource, accountingYear.Timbre, accountingYear.PourcentageFodec);

        accountingYear.UpdateAccountingYear(
            command.Year, 
            command.IsActive,
            command.Timbre,
            command.PourcentageFodec,
            command.VatRate0,
            command.VatRate7,
            command.VatRate13,
            command.VatRate19,
            command.PourcentageRetenu,
            command.VatAmount,
            command.SeuilRetenueSource);

        _logger.LogInformation("After UpdateAccountingYear - Entity values: VatAmount={VatAmount}, SeuilRetenueSource={SeuilRetenueSource}, Timbre={Timbre}, PourcentageFodec={PourcentageFodec}",
            accountingYear.VatAmount, accountingYear.SeuilRetenueSource, accountingYear.Timbre, accountingYear.PourcentageFodec);

        // Force EF Core to mark properties as modified
        var entry = _context.Entry(accountingYear);
        entry.Property("VatAmount").IsModified = true;
        entry.Property("SeuilRetenueSource").IsModified = true;
        entry.Property("Timbre").IsModified = true;
        entry.Property("PourcentageFodec").IsModified = true;
        entry.Property("VatRate0").IsModified = true;
        entry.Property("VatRate7").IsModified = true;
        entry.Property("VatRate13").IsModified = true;
        entry.Property("VatRate19").IsModified = true;
        entry.Property("PourcentageRetenu").IsModified = true;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("After SaveChanges - Entity values: VatAmount={VatAmount}, SeuilRetenueSource={SeuilRetenueSource}, Timbre={Timbre}, PourcentageFodec={PourcentageFodec}",
                accountingYear.VatAmount, accountingYear.SeuilRetenueSource, accountingYear.Timbre, accountingYear.PourcentageFodec);
            
            // Reload from database to verify persistence
            await _context.Entry(accountingYear).ReloadAsync(cancellationToken);
            _logger.LogInformation("After Reload from DB - Entity values: VatAmount={VatAmount}, SeuilRetenueSource={SeuilRetenueSource}, Timbre={Timbre}, PourcentageFodec={PourcentageFodec}",
                accountingYear.VatAmount, accountingYear.SeuilRetenueSource, accountingYear.Timbre, accountingYear.PourcentageFodec);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving accounting year {AccountingYearId}: {Message}", command.Id, ex.Message);
            throw;
        }

        // Si l'année mise à jour est active, invalider le cache des paramètres financiers
        if (accountingYear.IsActive)
        {
            _financialParametersService.InvalidateCache();
            _logger.LogInformation("Invalidated financial parameters cache because active accounting year was updated");
        }

        _logger.LogInformation("Accounting year {AccountingYearId} updated successfully", command.Id);

        var response = new AccountingYearResponse
        {
            Id = accountingYear.Id,
            Year = accountingYear.Year,
            IsActive = accountingYear.IsActive,
            Timbre = accountingYear.Timbre,
            PourcentageFodec = accountingYear.PourcentageFodec,
            VatRate0 = accountingYear.VatRate0,
            VatRate7 = accountingYear.VatRate7,
            VatRate13 = accountingYear.VatRate13,
            VatRate19 = accountingYear.VatRate19,
            PourcentageRetenu = accountingYear.PourcentageRetenu,
            VatAmount = accountingYear.VatAmount,
            SeuilRetenueSource = accountingYear.SeuilRetenueSource
        };

        return response;
    }
}
