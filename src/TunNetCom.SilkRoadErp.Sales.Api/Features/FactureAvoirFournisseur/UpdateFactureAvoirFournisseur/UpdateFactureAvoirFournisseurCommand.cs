using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.UpdateFactureAvoirFournisseur;

public record UpdateFactureAvoirFournisseurCommand(
    int Id,
    DateTime Date,
    int IdFournisseur,
    int NumFactureAvoirFourSurPage,
    int? FactureFournisseurId,
    List<int> AvoirFournisseurIds
) : IRequest<Result>;

