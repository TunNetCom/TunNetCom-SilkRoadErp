#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class PaiementClient : IAccountingYearEntity
{
    private PaiementClient()
    {
    }

    public static PaiementClient CreatePaiementClient(
        string? numeroTransactionBancaire,
        int clientId,
        int accountingYearId,
        decimal montant,
        DateTime datePaiement,
        MethodePaiement methodePaiement,
        IReadOnlyList<int>? factureIds,
        IReadOnlyList<int>? bonDeLivraisonIds,
        string? numeroChequeTraite,
        int? banqueId,
        DateTime? dateEcheance,
        string? commentaire,
        string? documentStoragePath)
    {
        return new PaiementClient
        {
            NumeroTransactionBancaire = numeroTransactionBancaire,
            ClientId = clientId,
            AccountingYearId = accountingYearId,
            Montant = montant,
            DatePaiement = datePaiement,
            MethodePaiement = methodePaiement,
            NumeroChequeTraite = numeroChequeTraite,
            BanqueId = banqueId,
            DateEcheance = dateEcheance,
            Commentaire = commentaire,
            DocumentStoragePath = documentStoragePath,
            Factures = new List<PaiementClientFacture>(),
            BonDeLivraisons = new List<PaiementClientBonDeLivraison>()
        };
    }

    public void UpdatePaiementClient(
        string? numeroTransactionBancaire,
        int clientId,
        int accountingYearId,
        decimal montant,
        DateTime datePaiement,
        MethodePaiement methodePaiement,
        IReadOnlyList<int>? factureIds,
        IReadOnlyList<int>? bonDeLivraisonIds,
        string? numeroChequeTraite,
        int? banqueId,
        DateTime? dateEcheance,
        string? commentaire,
        string? documentStoragePath)
    {
        this.NumeroTransactionBancaire = numeroTransactionBancaire;
        this.ClientId = clientId;
        this.AccountingYearId = accountingYearId;
        this.Montant = montant;
        this.DatePaiement = datePaiement;
        this.MethodePaiement = methodePaiement;
        this.NumeroChequeTraite = numeroChequeTraite;
        this.BanqueId = banqueId;
        this.DateEcheance = dateEcheance;
        this.Commentaire = commentaire;
        this.DocumentStoragePath = documentStoragePath;
        this.DateModification = DateTime.UtcNow;

        // Update Factures collection
        if (factureIds != null)
        {
            this.Factures.Clear();
            foreach (var factureId in factureIds)
            {
                this.Factures.Add(PaiementClientFacture.Create(this.Id, factureId));
            }
        }

        // Update BonDeLivraisons collection
        if (bonDeLivraisonIds != null)
        {
            this.BonDeLivraisons.Clear();
            foreach (var bonDeLivraisonId in bonDeLivraisonIds)
            {
                this.BonDeLivraisons.Add(PaiementClientBonDeLivraison.Create(this.Id, bonDeLivraisonId));
            }
        }
    }

    public int Id { get; private set; }

    public string? NumeroTransactionBancaire { get; private set; }

    public int ClientId { get; private set; }

    public int AccountingYearId { get; private set; }

    public decimal Montant { get; private set; }

    public DateTime DatePaiement { get; private set; }

    public MethodePaiement MethodePaiement { get; private set; }

    public string? NumeroChequeTraite { get; private set; }

    public int? BanqueId { get; private set; }

    public DateTime? DateEcheance { get; private set; }

    public string? Commentaire { get; private set; }

    public string? DocumentStoragePath { get; private set; }

    public DateTime? DateModification { get; private set; }

    public virtual Client Client { get; set; } = null!;

    public virtual AccountingYear AccountingYear { get; set; } = null!;

    public virtual Banque? Banque { get; set; }

    public virtual ICollection<PaiementClientFacture> Factures { get; set; } = new List<PaiementClientFacture>();

    public virtual ICollection<PaiementClientBonDeLivraison> BonDeLivraisons { get; set; } = new List<PaiementClientBonDeLivraison>();
}

