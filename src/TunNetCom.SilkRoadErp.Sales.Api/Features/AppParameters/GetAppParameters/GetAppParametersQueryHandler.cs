using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.CreateCustomer;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;

public class GetAppParametersQueryHandler(
    SalesContext _context,
    ILogger<CreateCustomerCommandHandler> _logger)
    : IRequestHandler<GetAppParametersQuery, Result<GetAppParametersResponse>>
{
    public async Task<Result<GetAppParametersResponse>> Handle(GetAppParametersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting app sales parameters from table {SalesAppParameterTable}", nameof(Systeme));

        var appParametersLines = await _context.Systeme.CountAsync(cancellationToken);

        if (appParametersLines == 0)
        {
            await _context.Systeme.AddAsync(new Systeme(), cancellationToken);
            _logger.LogInformation(
                "No app sales parameters found in table {SalesAppParameterTable}, then an empty line was created.",
                nameof(Systeme));

            return Result.Ok(new GetAppParametersResponse());
        }

        if (appParametersLines > 1)
        {
            throw new InvalidOperationException("There should be only one line in the app parameters table.");
        }

        var appParameters = await _context.Systeme.FirstOrDefaultAsync(cancellationToken);

        if (appParameters is null)
        {
            throw new InvalidOperationException("There should be only one line in the app parameters table.");
        }

        _logger.LogInformation(
            "App sales parameters fetched successfully from table {SalesAppParameterTable}",
            nameof(Systeme));

        return appParameters.Adapt<GetAppParametersResponse>();
    }
}
