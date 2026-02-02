namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.UpdatePaiementTiersDepense;

public record UpdatePaiementTiersDepenseCommand(
    int Id,
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
    int? Mois) : IRequest<Result>;
