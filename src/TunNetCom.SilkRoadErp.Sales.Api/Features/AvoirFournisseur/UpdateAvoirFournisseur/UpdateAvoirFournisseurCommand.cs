using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.UpdateAvoirFournisseur;

public record UpdateAvoirFournisseurCommand(
    int Num,
    DateTime Date,
    int? FournisseurId,
    int? NumFactureAvoirFournisseur,
    List<AvoirFournisseurLineRequest> Lines
) : IRequest<Result>;

