using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.CreatePaiementClient;

public record CreatePaiementClientCommand(
    string? NumeroTransactionBancaire,
    int ClientId,
    int? AccountingYearId,
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
) : IRequest<Result<int>>;

