#nullable enable
using System;
using System.Collections.Generic;

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
            Statut = DocumentStatus.Draft
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

    public void Valider()
    {
        if (Statut != DocumentStatus.Draft)
        {
            throw new InvalidOperationException("Seul un document en brouillon peut être validé.");
        }
        Statut = DocumentStatus.Valid;
    }

    public int Id { get; set; }

    public int Num { get; set; }

    public DateTime Date { get; set; }

    public int IdFournisseur { get; set; }

    public decimal TotHTva { get; set; }

    public decimal TotTva { get; set; }

    public decimal NetPayer { get; set; }

    public int AccountingYearId { get; set; }

    public DocumentStatus Statut { get; private set; }

    public virtual Fournisseur IdFournisseurNavigation { get; set; } = null!;

    public virtual ICollection<LigneRetourMarchandiseFournisseur> LigneRetourMarchandiseFournisseur { get; set; } = new List<LigneRetourMarchandiseFournisseur>();

    public virtual AccountingYear AccountingYear { get; set; } = null!;
}

