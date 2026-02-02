namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.CreatePaiementTiersDepense;

public record CreatePaiementTiersDepenseCommand(
    string? NumeroTransactionBancaire,
    int TiersDepenseFonctionnementId,
    int? AccountingYearId,
    decimal Montant,
    DateTime DatePaiement,
    string MethodePaiement,
    IReadOnlyList<int>? FactureDepenseIds,
    string? NumeroChequeTraite,
    int? BanqueId,
    DateTime? DateEcheance,
    string? Commentaire,
    string? RibCodeEtab,
    string? RibCodeAgence,
    string? RibNumeroCompte,
    string? RibCle,
    int? Mois) : IRequest<Result<int>>;
