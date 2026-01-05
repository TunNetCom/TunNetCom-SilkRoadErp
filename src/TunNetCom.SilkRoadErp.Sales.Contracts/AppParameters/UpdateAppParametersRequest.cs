using TunNetCom.SilkRoadErp.Sales.Contracts.Common;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

public class UpdateAppParametersRequest
{
    public UpdateAppParametersRequest()
    {

    }

    public UpdateAppParametersRequest(
        string nomSociete,
        string? adresse,
        string? tel,
        string? fax,
        string? email,
        string? matriculeFiscale,
        string? codeTva,
        string? codeCategorie,
        string? etbSecondaire,
        string? adresseRetenu,
        decimal? discountPercentage,
        bool? bloquerVenteStockInsuffisant,
        bool? bloquerBlSansFacture,
        int? decimalPlaces,
        string? rib)
    {
        NomSociete = nomSociete;
        Adresse = adresse;
        Tel = tel;
        Fax = fax;
        Email = email;
        MatriculeFiscale = matriculeFiscale;
        CodeTva = codeTva;
        CodeCategorie = codeCategorie;
        EtbSecondaire = etbSecondaire;
        AdresseRetenu = adresseRetenu;
        DiscountPercentage = discountPercentage ?? 0;
        BloquerVenteStockInsuffisant = bloquerVenteStockInsuffisant ?? true;
        BloquerBlSansFacture = bloquerBlSansFacture ?? false;
        DecimalPlaces = decimalPlaces ?? DecimalFormatConstants.DEFAULT_DECIMAL_PLACES;
        Rib = rib;
    }

    public string NomSociete { get; set; } = null!;

    public string Adresse { get; set; } = null!;

    public string Tel { get; set; } = null!;

    public string? Fax { get; set; }

    public string? Email { get; set; }

    public string? MatriculeFiscale { get; set; }

    public string CodeTva { get; set; } = null!;

    public string? CodeCategorie { get; set; }

    public string? EtbSecondaire { get; set; }

    public string? AdresseRetenu { get; set; }

    public decimal DiscountPercentage { get; set; }

    public bool BloquerVenteStockInsuffisant { get; set; }

    public bool BloquerBlSansFacture { get; set; }

    public int DecimalPlaces { get; set; } = DecimalFormatConstants.DEFAULT_DECIMAL_PLACES;

    public string? Rib { get; set; }
}
