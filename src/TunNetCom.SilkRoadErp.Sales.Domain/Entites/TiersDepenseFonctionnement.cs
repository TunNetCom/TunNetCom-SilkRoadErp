#nullable enable
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Tiers dédié aux dépenses de fonctionnement (non stockables).
/// Aucun lien avec BonDeReception, Commandes, Produit ou Inventaire.
/// </summary>
public partial class TiersDepenseFonctionnement
{
    private TiersDepenseFonctionnement()
    {
    }

    public static TiersDepenseFonctionnement Create(
        string nom,
        string? tel,
        string? adresse,
        string? matricule,
        string? code,
        string? codeCat,
        string? etbSec,
        string? mail,
        bool exonereRetenueSource = false)
    {
        return new TiersDepenseFonctionnement
        {
            Nom = nom,
            Tel = tel,
            Adresse = adresse,
            Matricule = matricule,
            Code = code,
            CodeCat = codeCat,
            EtbSec = etbSec,
            Mail = mail,
            ExonereRetenueSource = exonereRetenueSource
        };
    }

    public void Update(
        string nom,
        string? tel,
        string? adresse,
        string? matricule,
        string? code,
        string? codeCat,
        string? etbSec,
        string? mail,
        bool exonereRetenueSource = false)
    {
        Nom = nom;
        Tel = tel;
        Adresse = adresse;
        Matricule = matricule;
        Code = code;
        CodeCat = codeCat;
        EtbSec = etbSec;
        Mail = mail;
        ExonereRetenueSource = exonereRetenueSource;
    }

    public int Id { get; private set; }

    public string Nom { get; private set; } = null!;

    public string? Tel { get; private set; }

    public string? Adresse { get; private set; }

    public string? Matricule { get; private set; }

    public string? Code { get; private set; }

    public string? CodeCat { get; private set; }

    public string? EtbSec { get; private set; }

    public string? Mail { get; private set; }

    public bool ExonereRetenueSource { get; private set; }

    public virtual ICollection<FactureDepense> FactureDepense { get; set; } = new List<FactureDepense>();

    public virtual ICollection<PaiementTiersDepense> PaiementTiersDepense { get; set; } = new List<PaiementTiersDepense>();
}
