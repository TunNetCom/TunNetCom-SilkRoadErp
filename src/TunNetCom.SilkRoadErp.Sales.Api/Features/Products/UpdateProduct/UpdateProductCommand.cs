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
    int? SousFamilleProduitId = null,
    string? Image1Base64 = null,
    string? Image2Base64 = null,
    string? Image3Base64 = null,
    int? Id = null
) : IRequest<Result>;