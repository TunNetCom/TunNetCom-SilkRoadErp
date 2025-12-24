using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.UpdatePaiementClient;

public record UpdatePaiementClientCommand(
    int Id,
    string Numero,
    int ClientId,
    decimal Montant,
    DateTime DatePaiement,
    string MethodePaiement,
    List<int>? FactureIds,
    List<int>? BonDeLivraisonIds,
    string? NumeroChequeTraite,
    int? BanqueId,
    DateTime? DateEcheance,
    string? Commentaire,
    string? DocumentBase64
) : IRequest<Result>;

