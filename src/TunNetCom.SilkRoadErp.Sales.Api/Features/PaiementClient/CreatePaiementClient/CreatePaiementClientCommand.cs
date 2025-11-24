using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.CreatePaiementClient;

public record CreatePaiementClientCommand(
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
) : IRequest<Result<int>>;

