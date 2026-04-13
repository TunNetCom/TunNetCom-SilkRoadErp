using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.CreateCustomer;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;

public class GetAppParametersQueryHandler(
    SalesContext _context,
    ILogger<CreateCustomerCommandHandler> _logger,
    IAccountingYearFinancialParametersService _financialParametersService)
    : IRequestHandler<GetAppParametersQuery, Result<GetAppParametersResponse>>
{
    public async Task<Result<GetAppParametersResponse>> Handle(GetAppParametersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting app sales parameters from table {SalesAppParameterTable}", nameof(Systeme));

        var appParametersLines = await _context.Systeme.CountAsync(cancellationToken);

        if (appParametersLines == 0)
        {
            _ = await _context.Systeme.AddAsync(new Systeme(), cancellationToken);
            _logger.LogInformation(
                "No app sales parameters found in table {SalesAppParameterTable}, then an empty line was created.",
                nameof(Systeme));

            return Result.Ok(new GetAppParametersResponse());
        }

        if (appParametersLines > 1)
        {
            throw new InvalidOperationException("There should be only one line in the app parameters table.");
        }

        var appParameters = await _context.Systeme.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        if (appParameters is null)
        {
            throw new InvalidOperationException("There should be only one line in the app parameters table.");
        }

        // Get financial parameters from AccountingYear
        var financialParams = await _financialParametersService.GetAllFinancialParametersAsync(cancellationToken);

        _logger.LogInformation(
            "App sales parameters fetched successfully from table {SalesAppParameterTable}",
            nameof(Systeme));

        // Map Systeme to GetAppParametersResponse and add financial parameters from AccountingYear
        var response = appParameters.Adapt<GetAppParametersResponse>();
        
        // Override with values from AccountingYear if available
        // Note: DecimalPlaces remains in Systeme (not migrated)
        if (financialParams != null)
        {
            response.Timbre = financialParams.Timbre;
            response.PourcentageFodec = financialParams.PourcentageFodec;
            response.PourcentageRetenu = financialParams.PourcentageRetenu;
            response.VatRate0 = financialParams.VatRate0;
            response.VatRate7 = financialParams.VatRate7;
            response.VatRate13 = financialParams.VatRate13;
            response.VatRate19 = financialParams.VatRate19;
            response.VatAmount = financialParams.VatAmount;
            response.SeuilRetenueSource = financialParams.SeuilRetenueSource;
            // DecimalPlaces comes from Systeme (via financialParams which gets it from Systeme)
            response.DecimalPlaces = financialParams.DecimalPlaces;
        }

        return response;
    }
}
