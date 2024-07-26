namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.DeleteCustomer;

public class DeleteCustomerCommandHandler(SalesContext _context,
    ILogger<DeleteCustomerCommandHandler> _logger) : IRequestHandler<DeleteCustomerCommand, Result>
{
    public async Task<Result> Handle(DeleteCustomerCommand deleteCustomerCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityDeletionAttempt(
            nameof(Client),
            deleteCustomerCommand.Id);

        var client = await _context.Client.FindAsync(deleteCustomerCommand.Id);

        if (client is null)
        {
            _logger.LogEntityNotFound(nameof(Client), deleteCustomerCommand.Id);

            return Result.Fail("client_not_found");
        }

        _context.Client.Remove(client);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityDeleted(
            nameof(Client),
            deleteCustomerCommand.Id);

        return Result.Ok();
    }
}
