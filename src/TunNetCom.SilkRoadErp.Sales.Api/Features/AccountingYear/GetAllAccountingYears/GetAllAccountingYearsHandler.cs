namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.GetAllAccountingYears;

public class GetAllAccountingYearsHandler(
    SalesContext _context,
    ILogger<GetAllAccountingYearsHandler> _logger)
    : IRequestHandler<GetAllAccountingYearsQuery, Result<List<GetAllAccountingYearsResponse>>>
{
    public async Task<Result<List<GetAllAccountingYearsResponse>>> Handle(GetAllAccountingYearsQuery request, CancellationToken cancellationToken)
    {
        var years = await _context.AccountingYear
            .OrderByDescending(ay => ay.Year)
            .Select(ay => new GetAllAccountingYearsResponse(ay.Id, ay.Year, ay.IsActive))
            .ToListAsync(cancellationToken);

        return Result.Ok(years);
    }
}

