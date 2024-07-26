using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;

public class UpdateCustomerCommandHandler(
    SalesContext _context,
    ILogger<UpdateCustomerCommandHandler> _logger)
    : IRequestHandler<UpdateCustomerCommand, Result>
{
    public async Task<Result> Handle(UpdateCustomerCommand updateCustomerCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(Client), updateCustomerCommand.Id);

        var clientToUpdate = await _context.Client.FindAsync(updateCustomerCommand.Id);

        if (clientToUpdate is null)
        {
            _logger.LogEntityNotFound(nameof(Client), updateCustomerCommand.Id);

            return Result.Fail(EntityNotFound.Error);
        }

        var isCustomerNameExist = await _context.Client.AnyAsync(
            cus => cus.Nom == updateCustomerCommand.Nom
            && cus.Id != updateCustomerCommand.Id,
            cancellationToken);

        if (isCustomerNameExist)
        {
            return Result.Fail("customer_name_exist");
        }

        clientToUpdate.UpdateClient(
            nom: updateCustomerCommand.Nom,
            tel: updateCustomerCommand.Tel,
            adresse: updateCustomerCommand.Adresse,
            matricule: updateCustomerCommand.Matricule,
            code: updateCustomerCommand.Code,
            codeCat: updateCustomerCommand.CodeCat,
            etbSec: updateCustomerCommand.EtbSec,
            mail: updateCustomerCommand.Mail);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(Client), updateCustomerCommand.Id);

        return Result.Ok();
    }
}
