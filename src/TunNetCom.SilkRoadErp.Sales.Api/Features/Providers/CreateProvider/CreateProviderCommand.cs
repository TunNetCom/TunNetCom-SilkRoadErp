namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.CreateProvider;
public record CreateProviderCommand
(
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
    ) : IRequest<Result<int>>;
