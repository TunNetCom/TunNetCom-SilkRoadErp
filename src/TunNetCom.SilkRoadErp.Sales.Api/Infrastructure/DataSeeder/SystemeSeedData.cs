namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.DataSeeder;

public class SystemeSeedData
{
    public int Id { get; set; }
    public string NomSociete { get; set; } = null!;
    // Timbre, PourcentageFodec, PourcentageRetenu, VatRate0, VatRate7, VatRate13, VatRate19
    // ont été migrés vers AccountingYear et ne sont plus dans Systeme
    public string Adresse { get; set; } = null!;
    public string Tel { get; set; } = null!;
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? MatriculeFiscale { get; set; }
    public string CodeTva { get; set; } = null!;
    public string? CodeCategorie { get; set; }
    public string? EtbSecondaire { get; set; }
    public string? AdresseRetenu { get; set; }
    // VatAmount and SeuilRetenueSource have been migrated to AccountingYear
    public decimal DiscountPercentage { get; set; }
    public bool BloquerVenteStockInsuffisant { get; set; }
    public int DecimalPlaces { get; set; }
    public string? Rib { get; set; }
}


