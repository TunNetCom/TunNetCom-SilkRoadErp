using TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.CreateRetourMarchandiseFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.UpdateRetourMarchandiseFournisseur;

internal record class UpdateRetourMarchandiseFournisseurCommand(
    int Num,
    DateTime Date,
    int IdFournisseur,
    List<RetourMarchandiseFournisseurLigne> Lines
) : IRequest<Result>;

