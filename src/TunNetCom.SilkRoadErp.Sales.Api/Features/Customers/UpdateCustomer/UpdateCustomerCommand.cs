namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;

public record UpdateCustomerCommand(
    int Id,
    string Nom,
    string? Tel,
    string? Adresse,
    string? Matricule,
    string? Code,
    string? CodeCat,
    string? EtbSec,
    string? Mail) : IRequest<Result>;