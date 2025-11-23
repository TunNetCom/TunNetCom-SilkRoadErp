using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.UpdateFactureAvoirFournisseur;

public record UpdateFactureAvoirFournisseurCommand(
    int Num,
    DateTime Date,
    int IdFournisseur,
    int? NumFactureFournisseur,
    List<int> AvoirFournisseurIds
) : IRequest<Result>;

