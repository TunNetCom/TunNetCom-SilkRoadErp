#nullable enable
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class CompteBancaire
{
    private CompteBancaire()
    {
    }

    public static CompteBancaire CreateCompteBancaire(
        int banqueId,
        string codeEtablissement,
        string codeAgence,
        string numeroCompte,
        string cleRib,
        string? libelle = null)
    {
        return new CompteBancaire
        {
            BanqueId = banqueId,
            CodeEtablissement = codeEtablissement,
            CodeAgence = codeAgence,
            NumeroCompte = numeroCompte,
            CleRib = cleRib,
            Libelle = libelle
        };
    }

    public void UpdateCompteBancaire(
        int banqueId,
        string codeEtablissement,
        string codeAgence,
        string numeroCompte,
        string cleRib,
        string? libelle = null)
    {
        BanqueId = banqueId;
        CodeEtablissement = codeEtablissement;
        CodeAgence = codeAgence;
        NumeroCompte = numeroCompte;
        CleRib = cleRib;
        Libelle = libelle;
    }

    public int Id { get; private set; }

    public int BanqueId { get; private set; }

    public string CodeEtablissement { get; private set; } = null!;

    public string CodeAgence { get; private set; } = null!;

    public string NumeroCompte { get; private set; } = null!;

    public string CleRib { get; private set; } = null!;

    public string? Libelle { get; private set; }

    public virtual Banque Banque { get; set; } = null!;

    public virtual ICollection<BankTransactionImport> BankTransactionImport { get; set; } = new List<BankTransactionImport>();
}
