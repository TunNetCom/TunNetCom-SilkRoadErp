namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.UpdateProduct;

public record UpdateProductCommand(string? Refe,
    string? Nom,
    int QteLimite,
    double Remise,
    double RemiseAchat,
    double Tva,
    decimal Prix,
    decimal PrixAchat,
    bool Visibilite,
    int? SousFamilleProduitId = null
) : IRequest<Result>;