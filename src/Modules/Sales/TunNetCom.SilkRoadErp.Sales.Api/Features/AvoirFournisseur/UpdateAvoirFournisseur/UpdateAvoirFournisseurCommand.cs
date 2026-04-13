using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.UpdateAvoirFournisseur;

public record UpdateAvoirFournisseurCommand(
    int Id,
    DateTime Date,
    int? FournisseurId,
    int? NumFactureAvoirFournisseur,
    int NumAvoirChezFournisseur,
    List<AvoirFournisseurLineRequest> Lines
) : IRequest<Result>;

