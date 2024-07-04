namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.CreateClient;

public record CreateClientCommand(
    string Nom,
    string? Tel,
    string? Adresse,
    string? Matricule,
    string? Code,
    string? CodeCat,
    string? EtbSec,
    string? Mail
) : IRequest<Result<int>>;
