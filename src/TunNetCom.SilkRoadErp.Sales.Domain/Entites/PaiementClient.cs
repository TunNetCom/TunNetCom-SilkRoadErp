#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class PaiementClient
{
    private PaiementClient()
    {
    }

    public static PaiementClient CreatePaiementClient(
        string numero,
        int clientId,
        int accountingYearId,
        decimal montant,
        DateTime datePaiement,
        MethodePaiement methodePaiement,
        int? factureId,
        int? bonDeLivraisonId,
        string? numeroChequeTraite,
        int? banqueId,
        DateTime? dateEcheance,
        string? commentaire)
    {
        return new PaiementClient
        {
            Numero = numero,
            ClientId = clientId,
            AccountingYearId = accountingYearId,
            Montant = montant,
            DatePaiement = datePaiement,
            MethodePaiement = methodePaiement,
            FactureId = factureId,
            BonDeLivraisonId = bonDeLivraisonId,
            NumeroChequeTraite = numeroChequeTraite,
            BanqueId = banqueId,
            DateEcheance = dateEcheance,
            Commentaire = commentaire
        };
    }

    public void UpdatePaiementClient(
        string numero,
        int clientId,
        int accountingYearId,
        decimal montant,
        DateTime datePaiement,
        MethodePaiement methodePaiement,
        int? factureId,
        int? bonDeLivraisonId,
        string? numeroChequeTraite,
        int? banqueId,
        DateTime? dateEcheance,
        string? commentaire)
    {
        this.Numero = numero;
        this.ClientId = clientId;
        this.AccountingYearId = accountingYearId;
        this.Montant = montant;
        this.DatePaiement = datePaiement;
        this.MethodePaiement = methodePaiement;
        this.FactureId = factureId;
        this.BonDeLivraisonId = bonDeLivraisonId;
        this.NumeroChequeTraite = numeroChequeTraite;
        this.BanqueId = banqueId;
        this.DateEcheance = dateEcheance;
        this.Commentaire = commentaire;
        this.DateModification = DateTime.UtcNow;
    }

    public int Id { get; private set; }

    public string Numero { get; private set; } = null!;

    public int ClientId { get; private set; }

    public int AccountingYearId { get; private set; }

    public decimal Montant { get; private set; }

    public DateTime DatePaiement { get; private set; }

    public MethodePaiement MethodePaiement { get; private set; }

    public int? FactureId { get; private set; }

    public int? BonDeLivraisonId { get; private set; }

    public string? NumeroChequeTraite { get; private set; }

    public int? BanqueId { get; private set; }

    public DateTime? DateEcheance { get; private set; }

    public string? Commentaire { get; private set; }

    public DateTime? DateModification { get; private set; }

    public virtual Client Client { get; set; } = null!;

    public virtual AccountingYear AccountingYear { get; set; } = null!;

    public virtual Banque? Banque { get; set; }

    public virtual Facture? Facture { get; set; }

    public virtual BonDeLivraison? BonDeLivraison { get; set; }
}

