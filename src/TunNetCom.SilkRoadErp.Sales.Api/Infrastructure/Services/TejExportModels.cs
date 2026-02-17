using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

/// <summary>
/// Données d'une facture fournisseur prêtes pour l'export TEJ (montant avant retenue, ref certificat, etc.).
/// </summary>
public record ProviderInvoiceTejItem(
    FactureFournisseur FactureFournisseur,
    Fournisseur Fournisseur,
    decimal? MontantAvantRetenue,
    string RefCertif,
    string? NormalizedBeneficiaireMatricule,
    string? BeneficiaireTel8Digits);

/// <summary>
/// Données d'une facture dépense prêtes pour l'export TEJ (tiers, ref certificat, etc.).
/// </summary>
public record FactureDepenseTejItem(
    FactureDepense FactureDepense,
    TiersDepenseFonctionnement Tiers,
    string RefCertif,
    string? NormalizedBeneficiaireMatricule,
    string? BeneficiaireTel8Digits);
