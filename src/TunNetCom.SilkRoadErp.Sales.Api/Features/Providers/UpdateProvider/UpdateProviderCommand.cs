namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.UpdateProvider;
    public record UpdateProviderCommand(
    int Id,
    string Nom,
    string Tel,
    string? Fax,
    string? Matricule,
    string? Code,
    string? CodeCat,
    string? EtbSec,
    string? Mail,
    string? MailDeux,
    bool Constructeur,
    string? Adresse
    ) : IRequest<Result>;


