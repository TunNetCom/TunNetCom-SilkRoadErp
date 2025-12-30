#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class PaiementFournisseur : IAccountingYearEntity
{
    private PaiementFournisseur()
    {
    }

    public static PaiementFournisseur CreatePaiementFournisseur(
        string? numeroTransactionBancaire,
        int fournisseurId,
        int accountingYearId,
        decimal montant,
        DateTime datePaiement,
        MethodePaiement methodePaiement,
        IReadOnlyList<int>? factureFournisseurIds,
        IReadOnlyList<int>? bonDeReceptionIds,
        string? numeroChequeTraite,
        int? banqueId,
        DateTime? dateEcheance,
        string? commentaire,
        string? ribCodeEtab,
        string? ribCodeAgence,
        string? ribNumeroCompte,
        string? ribCle,
        string? documentStoragePath)
    {
        return new PaiementFournisseur
        {
            NumeroTransactionBancaire = numeroTransactionBancaire,
            FournisseurId = fournisseurId,
            AccountingYearId = accountingYearId,
            Montant = montant,
            DatePaiement = datePaiement,
            MethodePaiement = methodePaiement,
            NumeroChequeTraite = numeroChequeTraite,
            BanqueId = banqueId,
            DateEcheance = dateEcheance,
            Commentaire = commentaire,
            RibCodeEtab = ribCodeEtab,
            RibCodeAgence = ribCodeAgence,
            RibNumeroCompte = ribNumeroCompte,
            RibCle = ribCle,
            DocumentStoragePath = documentStoragePath,
            FactureFournisseurs = new List<PaiementFournisseurFactureFournisseur>(),
            BonDeReceptions = new List<PaiementFournisseurBonDeReception>()
        };
    }

    public void UpdatePaiementFournisseur(
        string? numeroTransactionBancaire,
        int fournisseurId,
        int accountingYearId,
        decimal montant,
        DateTime datePaiement,
        MethodePaiement methodePaiement,
        IReadOnlyList<int>? factureFournisseurIds,
        IReadOnlyList<int>? bonDeReceptionIds,
        string? numeroChequeTraite,
        int? banqueId,
        DateTime? dateEcheance,
        string? commentaire,
        string? ribCodeEtab,
        string? ribCodeAgence,
        string? ribNumeroCompte,
        string? ribCle,
        string? documentStoragePath)
    {
        this.NumeroTransactionBancaire = numeroTransactionBancaire;
        this.FournisseurId = fournisseurId;
        this.AccountingYearId = accountingYearId;
        this.Montant = montant;
        this.DatePaiement = datePaiement;
        this.MethodePaiement = methodePaiement;
        this.NumeroChequeTraite = numeroChequeTraite;
        this.BanqueId = banqueId;
        this.DateEcheance = dateEcheance;
        this.Commentaire = commentaire;
        this.RibCodeEtab = ribCodeEtab;
        this.RibCodeAgence = ribCodeAgence;
        this.RibNumeroCompte = ribNumeroCompte;
        this.RibCle = ribCle;
        this.DocumentStoragePath = documentStoragePath;
        this.DateModification = DateTime.UtcNow;

        // Update FactureFournisseurs collection
        if (factureFournisseurIds != null)
        {
            this.FactureFournisseurs.Clear();
            foreach (var factureFournisseurId in factureFournisseurIds)
            {
                this.FactureFournisseurs.Add(PaiementFournisseurFactureFournisseur.Create(this.Id, factureFournisseurId));
            }
        }

        // Update BonDeReceptions collection
        if (bonDeReceptionIds != null)
        {
            this.BonDeReceptions.Clear();
            foreach (var bonDeReceptionId in bonDeReceptionIds)
            {
                this.BonDeReceptions.Add(PaiementFournisseurBonDeReception.Create(this.Id, bonDeReceptionId));
            }
        }
    }

    public int Id { get; private set; }

    public string? NumeroTransactionBancaire { get; private set; }

    public int FournisseurId { get; private set; }

    public int AccountingYearId { get; private set; }

    public decimal Montant { get; private set; }

    public DateTime DatePaiement { get; private set; }

    public MethodePaiement MethodePaiement { get; private set; }

    public string? NumeroChequeTraite { get; private set; }

    public int? BanqueId { get; private set; }

    public DateTime? DateEcheance { get; private set; }

    public string? Commentaire { get; private set; }

    public string? RibCodeEtab { get; private set; }

    public string? RibCodeAgence { get; private set; }

    public string? RibNumeroCompte { get; private set; }

    public string? RibCle { get; private set; }

    public string? DocumentStoragePath { get; private set; }

    public DateTime? DateModification { get; private set; }

    public virtual Fournisseur Fournisseur { get; set; } = null!;

    public virtual AccountingYear AccountingYear { get; set; } = null!;

    public virtual Banque? Banque { get; set; }

    public virtual ICollection<PaiementFournisseurFactureFournisseur> FactureFournisseurs { get; set; } = new List<PaiementFournisseurFactureFournisseur>();

    public virtual ICollection<PaiementFournisseurBonDeReception> BonDeReceptions { get; set; } = new List<PaiementFournisseurBonDeReception>();
}

