namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.DataSeeder;

public class SystemeSeedData
{
    public int Id { get; set; }
    public string NomSociete { get; set; } = null!;
    public decimal Timbre { get; set; }
    public string Adresse { get; set; } = null!;
    public string Tel { get; set; } = null!;
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? MatriculeFiscale { get; set; }
    public string CodeTva { get; set; } = null!;
    public string? CodeCategorie { get; set; }
    public string? EtbSecondaire { get; set; }
    public decimal PourcentageFodec { get; set; }
    public string? AdresseRetenu { get; set; }
    public double PourcentageRetenu { get; set; }
    public decimal VatAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal VatRate0 { get; set; }
    public decimal VatRate7 { get; set; }
    public decimal VatRate13 { get; set; }
    public decimal VatRate19 { get; set; }
    public string? Rib { get; set; }
}


