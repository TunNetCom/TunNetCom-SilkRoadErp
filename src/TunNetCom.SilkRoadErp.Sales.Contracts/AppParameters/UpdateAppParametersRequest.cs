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
        double? pourcentageRetenu)
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
}
