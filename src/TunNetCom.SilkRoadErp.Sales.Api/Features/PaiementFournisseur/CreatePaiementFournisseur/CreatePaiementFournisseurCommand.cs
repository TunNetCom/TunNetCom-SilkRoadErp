using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.CreatePaiementFournisseur;

public record CreatePaiementFournisseurCommand(
    string Numero,
    int FournisseurId,
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
    string? DocumentBase64
) : IRequest<Result<int>>;

