namespace TunNetCom.SilkRoadErp.Sales.Contracts.Providers;
public class CreateProviderRequest
{
    public string Nom { get; set; } = null!;

    public string Tel { get; set; } = null!;

    public string? Fax { get; set; }

    public string? Matricule { get; set; }

    public string? Code { get; set; }

    public string? CodeCat { get; set; }

    public string? EtbSec { get; set; }

    public string? Mail { get; set; }

    public string? MailDeux { get; set; }

    public bool Constructeur { get; set; }

    public string? Adresse { get; set; }

    public double? TauxRetenu { get; set; }

    public bool ExonereRetenueSource { get; set; }

    public string? RibCodeEtab { get; set; }

    public string? RibCodeAgence { get; set; }

    public string? RibNumeroCompte { get; set; }

    public string? RibCle { get; set; }
}
