namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.CreatePriceQuote;

public record CreatePriceQuoteCommand(
     int IdClient,
     DateTime Date,
     decimal TotHTva,
     decimal TotTva,
     decimal TotTtc,
     IEnumerable<LigneDevisSubCommand> QuotationLines)
    : IRequest<Result<int>>;

public record LigneDevisSubCommand
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


