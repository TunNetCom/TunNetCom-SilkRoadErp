namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.CreateProduct;
public record CreateProductCommand(
    string? Refe,
    string? Nom,
    int Qte,
    int QteLimite,
    double Remise,
    double RemiseAchat,
    double Tva,
    decimal Prix,
    decimal PrixAchat,
    bool Visibilite
) : IRequest<Result<string>>;