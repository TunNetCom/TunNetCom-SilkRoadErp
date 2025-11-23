using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.CreateAvoirFournisseur;

public record CreateAvoirFournisseurCommand(
    DateTime Date,
    int? FournisseurId,
    int? NumFactureAvoirFournisseur,
    List<AvoirFournisseurLineRequest> Lines
) : IRequest<Result<int>>;

