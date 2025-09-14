namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.CreateCustomer;

public class CreateCustomerCommandHandler(
    SalesContext _context,
    ILogger<CreateCustomerCommandHandler> _logger)
    : IRequestHandler<CreateCustomerCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCustomerCommand createCustomerCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(Client),createCustomerCommand);

        var isCustomerNameExist = await _context.Client.AsNoTracking().AnyAsync(
            cus => cus.Nom == createCustomerCommand.Nom,
            cancellationToken);

        if (isCustomerNameExist)
        {
            return Result.Fail("customer_name_exist");
        }

        var client = Client.CreateClient
        (
            createCustomerCommand.Nom,
            createCustomerCommand.Tel,
            createCustomerCommand.Adresse,
            createCustomerCommand.Matricule,
            createCustomerCommand.Code,
            createCustomerCommand.CodeCat,
            createCustomerCommand.EtbSec,
            createCustomerCommand.Mail
        );


        _ = _context.Client.Add(client);
        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully(nameof(Client), client.Id);

        return client.Id;
    }
}
