#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class RetourMarchandiseFournisseur : IAccountingYearEntity
{
    public static RetourMarchandiseFournisseur CreateRetourMarchandiseFournisseur(
        int num,
        DateTime date,
        int idFournisseur,
        int accountingYearId,
        decimal totHTva,
        decimal totTva,
        decimal netPayer)
    {
        return new RetourMarchandiseFournisseur
        {
            Num = num,
            Date = date,
            IdFournisseur = idFournisseur,
            AccountingYearId = accountingYearId,
            TotHTva = totHTva,
            TotTva = totTva,
            NetPayer = netPayer,
            StatutRetour = RetourFournisseurStatus.Draft
        };
    }

    public void UpdateRetourMarchandiseFournisseur(
        int num,
        DateTime date,
        int idFournisseur,
        int accountingYearId,
        decimal totHTva,
        decimal totTva,
        decimal netPayer)
    {
        this.Num = num;
        this.Date = date;
        this.IdFournisseur = idFournisseur;
        this.AccountingYearId = accountingYearId;
        this.TotHTva = totHTva;
        this.TotTva = totTva;
        this.NetPayer = netPayer;
    }

    /// <summary>
    /// Valide le bon de retour (passe de brouillon à validé)
    /// </summary>
    public void Valider()
    {
        if (StatutRetour != RetourFournisseurStatus.Draft)
        {
            throw new InvalidOperationException("Seul un document en brouillon peut être validé.");
        }
        StatutRetour = RetourFournisseurStatus.Valid;
    }

    /// <summary>
    /// Passe le retour en statut "En réparation" (les produits sont chez le fournisseur)
    /// </summary>
    public void PasserEnReparation()
    {
        if (StatutRetour != RetourFournisseurStatus.Valid)
        {
            throw new InvalidOperationException("Seul un retour validé peut passer en réparation.");
        }
        StatutRetour = RetourFournisseurStatus.EnReparation;
    }

    /// <summary>
    /// Valide la réception après réparation et met à jour le statut en fonction des quantités reçues
    /// </summary>
    public void ValiderReception()
    {
        if (StatutRetour != RetourFournisseurStatus.Valid && 
            StatutRetour != RetourFournisseurStatus.EnReparation &&
            StatutRetour != RetourFournisseurStatus.ReceptionPartielle)
        {
            throw new InvalidOperationException("La réception ne peut être validée que pour un retour validé, en réparation ou en réception partielle.");
        }

        // Vérifier si toutes les quantités ont été reçues
        var toutesLignesCompletes = LigneRetourMarchandiseFournisseur.All(l => l.QteRecue >= l.QteLi);
        var auMoinsUneLigneRecue = LigneRetourMarchandiseFournisseur.Any(l => l.QteRecue > 0);

        if (toutesLignesCompletes)
        {
            StatutRetour = RetourFournisseurStatus.Cloture;
        }
        else if (auMoinsUneLigneRecue)
        {
            StatutRetour = RetourFournisseurStatus.ReceptionPartielle;
        }
        else
        {
            StatutRetour = RetourFournisseurStatus.EnReparation;
        }
    }

    /// <summary>
    /// Calcule la quantité totale encore en attente de réception
    /// </summary>
    public int GetQuantiteEnAttenteReception()
    {
        return LigneRetourMarchandiseFournisseur.Sum(l => Math.Max(0, l.QteLi - l.QteRecue));
    }

    /// <summary>
    /// Calcule la quantité totale déjà reçue
    /// </summary>
    public int GetQuantiteTotaleRecue()
    {
        return LigneRetourMarchandiseFournisseur.Sum(l => l.QteRecue);
    }

    public int Id { get; set; }

    public int Num { get; set; }

    public DateTime Date { get; set; }

    public int IdFournisseur { get; set; }

    public decimal TotHTva { get; set; }

    public decimal TotTva { get; set; }

    public decimal NetPayer { get; set; }

    public int AccountingYearId { get; set; }

    /// <summary>
    /// Statut du retour fournisseur avec le workflow complet
    /// </summary>
    public RetourFournisseurStatus StatutRetour { get; private set; }

    /// <summary>
    /// Propriété de compatibilité avec l'ancien système (convertit vers/depuis RetourFournisseurStatus)
    /// </summary>
    [Obsolete("Utiliser StatutRetour à la place")]
    public DocumentStatus Statut
    {
        get => StatutRetour <= RetourFournisseurStatus.Valid 
            ? (DocumentStatus)(int)StatutRetour 
            : DocumentStatus.Valid;
        private set => StatutRetour = (RetourFournisseurStatus)(int)value;
    }

    public virtual Fournisseur IdFournisseurNavigation { get; set; } = null!;

    public virtual ICollection<LigneRetourMarchandiseFournisseur> LigneRetourMarchandiseFournisseur { get; set; } = new List<LigneRetourMarchandiseFournisseur>();

    public virtual ICollection<ReceptionRetourFournisseur> ReceptionRetourFournisseur { get; set; } = new List<ReceptionRetourFournisseur>();

    public virtual AccountingYear AccountingYear { get; set; } = null!;
}
