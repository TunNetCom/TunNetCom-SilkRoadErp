namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.GetActiveAccountingYear;

public class GetActiveAccountingYearHandler(
    SalesContext _context,
    ILogger<GetActiveAccountingYearHandler> _logger)
    : IRequestHandler<GetActiveAccountingYearQuery, Result<GetActiveAccountingYearResponse>>
{
    public async Task<Result<GetActiveAccountingYearResponse>> Handle(GetActiveAccountingYearQuery request, CancellationToken cancellationToken)
    {
        var activeYear = await _context.AccountingYear
            .IgnoreQueryFilters() // Ignorer les filtres globaux pour accéder à tous les exercices
            .Where(ay => ay.IsActive)
            .Select(ay => new GetActiveAccountingYearResponse(ay.Id, ay.Year, ay.IsActive))
            .FirstOrDefaultAsync(cancellationToken);

        if (activeYear == null)
        {
            _logger.LogWarning("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        return Result.Ok(activeYear);
    }
}

