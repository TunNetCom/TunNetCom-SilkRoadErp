namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;

public class GetCustomerByIdQueryHandler(
    SalesContext _context,
    ILogger<GetCustomerByIdQueryHandler> _logger)
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerResponse>>
{
    public async Task<Result<CustomerResponse>> Handle(GetCustomerByIdQuery getClientByIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(Client), getClientByIdQuery.Id);

        var client = await _context.Client.FindAsync(getClientByIdQuery.Id, cancellationToken);

        if (client is null)
        {
            _logger.LogEntityNotFound(nameof(Client), getClientByIdQuery.Id);

            return Result.Fail("client_not_found");
        }

        _logger.LogEntityFetchedById(nameof(Client), getClientByIdQuery.Id);

        return client.Adapt<CustomerResponse>();
    }
}