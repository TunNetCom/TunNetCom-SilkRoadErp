using MediatR;


namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;

public class GetCustomerByIdQueryHandler(
    SalesContext _context,
    ILogger<GetCustomerByIdQueryHandler> _logger)
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerResponse>>
{
    public async Task<Result<CustomerResponse>> Handle(
        GetCustomerByIdQuery getClientByIdQuery,
        CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(Client), getClientByIdQuery.Id);

        var client = await _context.Client.FindAsync(getClientByIdQuery.Id, cancellationToken);

        if (client is null)
        {
            _logger.LogEntityNotFound(nameof(Client), getClientByIdQuery.Id);

            return Result.Fail(EntityNotFound.Error());
        }

        _logger.LogEntityFetchedById(nameof(Client), getClientByIdQuery.Id);

        return new CustomerResponse
        {
            Id = client.Id,
            Name = client.Nom, 
            Tel = client.Tel,
            Adresse = client.Adresse,
            Matricule = client.Matricule,
            Code = client.Code,
            CodeCat = client.CodeCat,
            EtbSec = client.EtbSec,
            Mail = client.Mail
        };
    }
}