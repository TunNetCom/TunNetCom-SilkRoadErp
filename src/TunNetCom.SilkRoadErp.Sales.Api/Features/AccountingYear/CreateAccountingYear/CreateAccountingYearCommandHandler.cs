using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.CreateAccountingYear;

public class CreateAccountingYearCommandHandler(
    SalesContext _context,
    ILogger<CreateAccountingYearCommandHandler> _logger)
    : IRequestHandler<CreateAccountingYearCommand, Result<AccountingYearResponse>>
{
    public async Task<Result<AccountingYearResponse>> Handle(CreateAccountingYearCommand command, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(Domain.Entites.AccountingYear), command);

        // Vérifier l'unicité de l'année
        var yearExists = await _context.AccountingYear
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(ay => ay.Year == command.Year, cancellationToken);

        if (yearExists)
        {
            return Result.Fail("accounting_year_already_exists");
        }

        // Si on active cet exercice, désactiver tous les autres
        if (command.IsActive)
        {
            var allYears = await _context.AccountingYear
                .IgnoreQueryFilters()
                .Where(ay => ay.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var year in allYears)
            {
                year.SetInactive();
            }
        }

        var accountingYear = Domain.Entites.AccountingYear.CreateAccountingYear(command.Year, command.IsActive);

        _context.AccountingYear.Add(accountingYear);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully(nameof(Domain.Entites.AccountingYear), accountingYear.Id);

        return new AccountingYearResponse
        {
            Id = accountingYear.Id,
            Year = accountingYear.Year,
            IsActive = accountingYear.IsActive
        };
    }
}
