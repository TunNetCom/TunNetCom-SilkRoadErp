namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.CreateOrder;

public record CreateOrderCommand(
    int? FournisseurId,
    DateTime Date,
    decimal TotHTva,
    decimal TotTva,
    decimal TotTtc,
    IEnumerable<LigneCommandeSubCommand> OrderLines)
    : IRequest<Result<int>>;

public record LigneCommandeSubCommand
{
    public string RefProduit { get; set; } = string.Empty;

    public string DesignationLi { get; set; } = string.Empty;

    public int QteLi { get; set; }

    public decimal PrixHt { get; set; }

    public double Remise { get; set; }

    public decimal TotHt { get; set; }

    public double Tva { get; set; }

    public decimal TotTtc { get; set; }
}

