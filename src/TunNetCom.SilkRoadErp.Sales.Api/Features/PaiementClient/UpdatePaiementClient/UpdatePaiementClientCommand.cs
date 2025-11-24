using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.UpdatePaiementClient;

public record UpdatePaiementClientCommand(
    int Id,
    string Numero,
    int ClientId,
    decimal Montant,
    DateTime DatePaiement,
    string MethodePaiement,
    int? FactureId,
    int? BonDeLivraisonId,
    string? NumeroChequeTraite,
    int? BanqueId,
    DateTime? DateEcheance,
    string? Commentaire
) : IRequest<Result>;

