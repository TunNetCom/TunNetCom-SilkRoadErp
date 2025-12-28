namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.UpdateAppParameters;
public record UpdateAppParametersCommand(
    string NomSociete,
    string? Adresse,
    string? Tel,
    string? Fax,
    string? Email,
    string? MatriculeFiscale,
    string? CodeTva,
    string? CodeCategorie,
    string? EtbSecondaire,
    string? AdresseRetenu,
    decimal? DiscountPercentage,
    bool? BloquerVenteStockInsuffisant,
    int? DecimalPlaces,
    string? Rib) : IRequest<Result>;