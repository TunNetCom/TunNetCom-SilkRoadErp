namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.CreateRetourMarchandiseFournisseur;

internal record class CreateRetourMarchandiseFournisseurCommand(
    DateTime Date,
    int IdFournisseur,
    List<RetourMarchandiseFournisseurLigne> Lines
) : IRequest<Result<int>>;

internal record class RetourMarchandiseFournisseurLigne(
    string ProductRef,
    string Description,
    int Quantity,
    decimal UnitPrice,
    double Discount,
    double Tax);

