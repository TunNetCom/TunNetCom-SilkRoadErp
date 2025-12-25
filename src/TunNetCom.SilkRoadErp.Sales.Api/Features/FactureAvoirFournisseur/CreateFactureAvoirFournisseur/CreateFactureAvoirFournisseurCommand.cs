using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.CreateFactureAvoirFournisseur;

public record CreateFactureAvoirFournisseurCommand(
    DateTime Date,
    int IdFournisseur,
    int NumFactureAvoirFourSurPage,
    int? FactureFournisseurId,
    List<int> AvoirFournisseurIds
) : IRequest<Result<int>>;

