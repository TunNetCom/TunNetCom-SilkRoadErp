namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.CreateCustomer;

public record CreateCustomerCommand(
    string Nom,
    string? Tel,
    string? Adresse,
    string? Matricule,
    string? Code,
    string? CodeCat,
    string? EtbSec,
    string? Mail
) : IRequest<Result<int>>;
