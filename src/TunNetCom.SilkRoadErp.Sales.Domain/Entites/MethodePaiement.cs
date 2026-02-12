namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public enum MethodePaiement
{
    Espece,
    Cheque,
    Traite,
    Virement,
    Tpe
}

/// <summary>
/// Constantes des noms de méthodes de paiement (alignées sur l'enum MethodePaiement).
/// À utiliser au lieu de chaînes en dur pour les comparaisons et valeurs envoyées à l'API.
/// </summary>
public static class MethodePaiementConsts
{
    public const string Espece = nameof(MethodePaiement.Espece);
    public const string Cheque = nameof(MethodePaiement.Cheque);
    public const string Traite = nameof(MethodePaiement.Traite);
    public const string Virement = nameof(MethodePaiement.Virement);
    public const string Tpe = nameof(MethodePaiement.Tpe);
}

