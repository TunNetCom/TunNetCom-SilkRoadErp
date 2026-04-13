using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.GetAccountingYearById;

public class GetAccountingYearByIdQueryHandler(
    SalesContext _context,
    ILogger<GetAccountingYearByIdQueryHandler> _logger)
    : IRequestHandler<GetAccountingYearByIdQuery, Result<AccountingYearResponse>>
{
    public async Task<Result<AccountingYearResponse>> Handle(GetAccountingYearByIdQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting accounting year with Id {AccountingYearId}", query.Id);

        var accountingYear = await _context.AccountingYear
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(ay => ay.Id == query.Id, cancellationToken);

        if (accountingYear == null)
        {
            return Result.Fail("accounting_year_not_found");
        }

        return new AccountingYearResponse
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
    }
}
