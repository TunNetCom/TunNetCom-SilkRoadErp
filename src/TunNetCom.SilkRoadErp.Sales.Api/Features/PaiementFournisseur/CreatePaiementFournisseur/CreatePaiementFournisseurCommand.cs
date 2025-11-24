using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.CreatePaiementFournisseur;

public record CreatePaiementFournisseurCommand(
    string Numero,
    int FournisseurId,
    decimal Montant,
    DateTime DatePaiement,
    string MethodePaiement,
    int? FactureFournisseurId,
    int? BonDeReceptionId,
    string? NumeroChequeTraite,
    int? BanqueId,
    DateTime? DateEcheance,
    string? Commentaire
) : IRequest<Result<int>>;

