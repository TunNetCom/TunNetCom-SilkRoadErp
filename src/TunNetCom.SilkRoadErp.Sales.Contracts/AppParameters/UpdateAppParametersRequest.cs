namespace TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

public class UpdateAppParametersRequest
{
    public UpdateAppParametersRequest()
    {

    }

    public UpdateAppParametersRequest(
        string nomSociete,
        decimal? timbre,
        string? adresse,
        string? tel,
        string? fax,
        string? email,
        string? matriculeFiscale,
        string? codeTva,
        string? codeCategorie,
        string? etbSecondaire,
        decimal? pourcentageFodec,
        string? adresseRetenu,
        double? pourcentageRetenu,
        decimal? vatAmount,
        decimal? discountPercentage,
        decimal? vatRate0,
        decimal? vatRate7,
        decimal? vatRate13,
        decimal? vatRate19)
    {
        NomSociete = nomSociete;
        Timbre = (decimal)timbre;
        Adresse = adresse;
        Tel = tel;
        Fax = fax;
        Email = email;
        MatriculeFiscale = matriculeFiscale;
        CodeTva = codeTva;
        CodeCategorie = codeCategorie;
        EtbSecondaire = etbSecondaire;
        PourcentageFodec = (decimal)pourcentageFodec;
        AdresseRetenu = adresseRetenu;
        PourcentageRetenu = (double)pourcentageRetenu;
        VatAmount = vatAmount ?? 0;
        DiscountPercentage = discountPercentage ?? 0;
        VatRate0 = vatRate0 ?? 0;
        VatRate7 = vatRate7 ?? 7;
        VatRate13 = vatRate13 ?? 13;
        VatRate19 = vatRate19 ?? 19;
    }

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

    public decimal DiscountPercentage { get; set; }

    public decimal VatAmount { get; set; }

    public decimal VatRate0 { get; set; }

    public decimal VatRate7 { get; set; }

    public decimal VatRate13 { get; set; }

    public decimal VatRate19 { get; set; }
}
