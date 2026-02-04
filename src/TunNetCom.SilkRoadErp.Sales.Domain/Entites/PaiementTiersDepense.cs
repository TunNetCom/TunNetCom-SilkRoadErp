#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Paiement pour un tiers dépenses de fonctionnement.
/// </summary>
public partial class PaiementTiersDepense : IAccountingYearEntity
{
    private PaiementTiersDepense()
    {
    }

    public static PaiementTiersDepense Create(
        string? numeroTransactionBancaire,
        int tiersDepenseFonctionnementId,
        int accountingYearId,
        decimal montant,
        DateTime datePaiement,
        MethodePaiement methodePaiement,
        IReadOnlyList<int>? factureDepenseIds,
        string? numeroChequeTraite,
        int? banqueId,
        DateTime? dateEcheance,
        string? commentaire,
        string? ribCodeEtab,
        string? ribCodeAgence,
        string? ribNumeroCompte,
        string? ribCle,
        string? documentStoragePath,
        int? mois)
    {
        var entity = new PaiementTiersDepense
        {
            NumeroTransactionBancaire = numeroTransactionBancaire,
            TiersDepenseFonctionnementId = tiersDepenseFonctionnementId,
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
            Mois = mois,
            FactureDepenses = new List<PaiementTiersDepenseFactureDepense>()
        };
        return entity;
    }

    public void Update(
        string? numeroTransactionBancaire,
        int tiersDepenseFonctionnementId,
        int accountingYearId,
        decimal montant,
        DateTime datePaiement,
        MethodePaiement methodePaiement,
        IReadOnlyList<int>? factureDepenseIds,
        string? numeroChequeTraite,
        int? banqueId,
        DateTime? dateEcheance,
        string? commentaire,
        string? ribCodeEtab,
        string? ribCodeAgence,
        string? ribNumeroCompte,
        string? ribCle,
        string? documentStoragePath,
        int? mois)
    {
        NumeroTransactionBancaire = numeroTransactionBancaire;
        TiersDepenseFonctionnementId = tiersDepenseFonctionnementId;
        AccountingYearId = accountingYearId;
        Montant = montant;
        DatePaiement = datePaiement;
        MethodePaiement = methodePaiement;
        NumeroChequeTraite = numeroChequeTraite;
        BanqueId = banqueId;
        DateEcheance = dateEcheance;
        Commentaire = commentaire;
        RibCodeEtab = ribCodeEtab;
        RibCodeAgence = ribCodeAgence;
        RibNumeroCompte = ribNumeroCompte;
        RibCle = ribCle;
        DocumentStoragePath = documentStoragePath;
        Mois = mois;
        DateModification = DateTime.UtcNow;

        if (factureDepenseIds != null)
        {
            FactureDepenses.Clear();
            foreach (var factureDepenseId in factureDepenseIds)
            {
                FactureDepenses.Add(PaiementTiersDepenseFactureDepense.Create(Id, factureDepenseId));
            }
        }
    }

    public int Id { get; private set; }

    public string? NumeroTransactionBancaire { get; private set; }

    public int TiersDepenseFonctionnementId { get; private set; }

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

    public int? Mois { get; private set; }

    public string? DocumentStoragePath { get; private set; }

    public DateTime? DateModification { get; private set; }

    public virtual TiersDepenseFonctionnement TiersDepenseFonctionnement { get; set; } = null!;

    public virtual AccountingYear AccountingYear { get; set; } = null!;

    public virtual Banque? Banque { get; set; }

    public virtual ICollection<PaiementTiersDepenseFactureDepense> FactureDepenses { get; set; } = new List<PaiementTiersDepenseFactureDepense>();
}
