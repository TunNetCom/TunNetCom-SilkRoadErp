namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public record UpdateClientCommand(
    int Id
    , string Nom
    , string? Tel
    , string? Adresse
    , string? Matricule
    , string? Code
    , string? CodeCat
    , string? EtbSec
    , string? Mail) : IRequest;