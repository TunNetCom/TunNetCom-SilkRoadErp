namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class Produit
{
    public Produit(string refe, string nom, int qte, int qteLimite, double remise, double remiseAchat, double tva, decimal prix, decimal prixAchat, bool visibilite)
    {
        Refe = refe;
        Nom = nom;
        Qte = qte;
        QteLimite = qteLimite;
        Remise = remise;
        RemiseAchat = remiseAchat;
        Tva = tva;
        Prix = prix;
        PrixAchat = prixAchat;
        Visibilite = visibilite;
    }

    public static Produit CreateProduct(
     string? refe,
     string? nom,
     int qte,
     int qteLimite,
     double remise,
     double remiseAchat,
     double tva,
     decimal prix,
     decimal prixAchat,
     bool visibilite)
    {
        return new Produit
        (
            refe: refe,
            nom: nom,
            qte: qte,
            qteLimite: qteLimite,
            remise: remise,
            remiseAchat: remiseAchat,
            tva: tva,
            prix: prix,
            prixAchat: prixAchat,
            visibilite: visibilite
        );

    }
    public void UpdateProduct(
        string? refe,
        string? nom,
        int qte,
        int qteLimite,
        double remise,
        double remiseAchat,
        double tva,
        decimal prix,
        decimal prixAchat,
        bool visibilite)
    {

        Refe = refe;
        Nom = nom;
        Qte = qte;
        QteLimite = qteLimite;
        Remise = remise;
        RemiseAchat = remiseAchat;
        Tva = tva;
        Prix = prix;
        PrixAchat = prixAchat;
        Visibilite = visibilite;

    }
}
