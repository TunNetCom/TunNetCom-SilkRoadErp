namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.PaiementFournisseur.PrintTraite;

public class PrintTraiteModel
{
    // Informations du paiement
    public int PaiementId { get; set; }
    public string NumeroPaiement { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime? DateEcheance { get; set; }
    public string? NumeroChequeTraite { get; set; }
    
    // Informations du tireur (société/vendeur)
    public TireurModel Tireur { get; set; } = null!;
    
    // Informations du tiré (fournisseur/acheteur)
    public TireModel Tire { get; set; } = null!;
    
    // Informations de la banque
    public BanqueModel? Banque { get; set; }
    
    // Montant en lettres
    public string MontantEnLettres { get; set; } = string.Empty;
}

public class TireurModel
{
    public string Nom { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? Tel { get; set; }
    public string? MatriculeFiscale { get; set; }
    public string? CodeTva { get; set; }
    public string? CodeCategorie { get; set; }
    public string? EtbSecondaire { get; set; }
}

public class TireModel
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? Tel { get; set; }
    public string? Matricule { get; set; }
    public string? Code { get; set; }
    
    // RIB/RIP du tiré (format: CodeEtab | CodeAgence | NumeroCompte | Cle)
    public string? RibCodeEtab { get; set; }
    public string? RibCodeAgence { get; set; }
    public string? RibNumeroCompte { get; set; }
    public string? RibCle { get; set; }
    
    public string RibFormatted => string.IsNullOrWhiteSpace(RibCodeEtab) 
        ? string.Empty 
        : $"{RibCodeEtab} | {RibCodeAgence} | {RibNumeroCompte} | {RibCle}";
}

public class BanqueModel
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Adresse { get; set; }
}






