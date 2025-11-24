#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class PaiementFournisseur
{
    private PaiementFournisseur()
    {
    }

    public static PaiementFournisseur CreatePaiementFournisseur(
        string numero,
        int fournisseurId,
        int accountingYearId,
        decimal montant,
        DateTime datePaiement,
        MethodePaiement methodePaiement,
        int? factureFournisseurId,
        int? bonDeReceptionId,
        string? numeroChequeTraite,
        int? banqueId,
        DateTime? dateEcheance,
        string? commentaire)
    {
        return new PaiementFournisseur
        {
            Numero = numero,
            FournisseurId = fournisseurId,
            AccountingYearId = accountingYearId,
            Montant = montant,
            DatePaiement = datePaiement,
            MethodePaiement = methodePaiement,
            FactureFournisseurId = factureFournisseurId,
            BonDeReceptionId = bonDeReceptionId,
            NumeroChequeTraite = numeroChequeTraite,
            BanqueId = banqueId,
            DateEcheance = dateEcheance,
            Commentaire = commentaire
        };
    }

    public void UpdatePaiementFournisseur(
        string numero,
        int fournisseurId,
        int accountingYearId,
        decimal montant,
        DateTime datePaiement,
        MethodePaiement methodePaiement,
        int? factureFournisseurId,
        int? bonDeReceptionId,
        string? numeroChequeTraite,
        int? banqueId,
        DateTime? dateEcheance,
        string? commentaire)
    {
        this.Numero = numero;
        this.FournisseurId = fournisseurId;
        this.AccountingYearId = accountingYearId;
        this.Montant = montant;
        this.DatePaiement = datePaiement;
        this.MethodePaiement = methodePaiement;
        this.FactureFournisseurId = factureFournisseurId;
        this.BonDeReceptionId = bonDeReceptionId;
        this.NumeroChequeTraite = numeroChequeTraite;
        this.BanqueId = banqueId;
        this.DateEcheance = dateEcheance;
        this.Commentaire = commentaire;
        this.DateModification = DateTime.UtcNow;
    }

    public int Id { get; private set; }

    public string Numero { get; private set; } = null!;

    public int FournisseurId { get; private set; }

    public int AccountingYearId { get; private set; }

    public decimal Montant { get; private set; }

    public DateTime DatePaiement { get; private set; }

    public MethodePaiement MethodePaiement { get; private set; }

    public int? FactureFournisseurId { get; private set; }

    public int? BonDeReceptionId { get; private set; }

    public string? NumeroChequeTraite { get; private set; }

    public int? BanqueId { get; private set; }

    public DateTime? DateEcheance { get; private set; }

    public string? Commentaire { get; private set; }

    public DateTime? DateModification { get; private set; }

    public virtual Fournisseur Fournisseur { get; set; } = null!;

    public virtual AccountingYear AccountingYear { get; set; } = null!;

    public virtual Banque? Banque { get; set; }

    public virtual FactureFournisseur? FactureFournisseur { get; set; }

    public virtual BonDeReception? BonDeReception { get; set; }
}

