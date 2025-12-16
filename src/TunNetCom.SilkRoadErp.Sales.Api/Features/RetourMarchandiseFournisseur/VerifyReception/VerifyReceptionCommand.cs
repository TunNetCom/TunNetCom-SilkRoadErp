using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.VerifyReception;

public record VerifyReceptionCommand(
    int Num,
    List<VerifyReceptionLineRequest> Lines,
    string Utilisateur,
    string? Commentaire
) : IRequest<Result<VerifyReceptionResponse>>;
