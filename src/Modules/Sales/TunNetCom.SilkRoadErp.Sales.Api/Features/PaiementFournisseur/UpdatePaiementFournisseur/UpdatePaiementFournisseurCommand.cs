using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.UpdatePaiementFournisseur;

public record UpdatePaiementFournisseurCommand(
    int Id,
    string? NumeroTransactionBancaire,
    int FournisseurId,
    int? AccountingYearId,
    decimal Montant,
    DateTime DatePaiement,
    string MethodePaiement,
    List<int>? FactureFournisseurIds,
    List<int>? BonDeReceptionIds,
    string? NumeroChequeTraite,
    int? BanqueId,
    DateTime? DateEcheance,
    string? Commentaire,
    string? RibCodeEtab,
    string? RibCodeAgence,
    string? RibNumeroCompte,
    string? RibCle,
    int? Mois,
    string? DocumentBase64
) : IRequest<Result>;

