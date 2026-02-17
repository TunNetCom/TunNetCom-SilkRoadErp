#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Facture de dépense avec 4 lignes TVA (0%, 7%, 13%, 19%). Saisie simple de BaseHT et MontantTVA par taux.
/// </summary>
public partial class FactureDepense : IAccountingYearEntity
{
    private FactureDepense()
    {
    }

    public static FactureDepense Create(
        int num,
        int idTiersDepenseFonctionnement,
        DateTime date,
        string description,
        decimal montantTotal,
        int accountingYearId,
        string? documentStoragePath = null,
        decimal? baseHT0 = null,
        decimal? montantTVA0 = null,
        decimal? baseHT7 = null,
        decimal? montantTVA7 = null,
        decimal? baseHT13 = null,
        decimal? montantTVA13 = null,
        decimal? baseHT19 = null,
        decimal? montantTVA19 = null)
    {
        return new FactureDepense
        {
            Num = num,
            IdTiersDepenseFonctionnement = idTiersDepenseFonctionnement,
            Date = date,
            Description = description ?? string.Empty,
            MontantTotal = montantTotal,
            BaseHT0 = baseHT0 ?? 0,
            MontantTVA0 = montantTVA0 ?? 0,
            BaseHT7 = baseHT7 ?? 0,
            MontantTVA7 = montantTVA7 ?? 0,
            BaseHT13 = baseHT13 ?? 0,
            MontantTVA13 = montantTVA13 ?? 0,
            BaseHT19 = baseHT19 ?? 0,
            MontantTVA19 = montantTVA19 ?? 0,
            AccountingYearId = accountingYearId,
            Statut = DocumentStatus.Draft,
            DocumentStoragePath = documentStoragePath
        };
    }

    public void Update(
        DateTime date,
        string description,
        decimal montantTotal,
        string? documentStoragePath,
        decimal? baseHT0 = null,
        decimal? montantTVA0 = null,
        decimal? baseHT7 = null,
        decimal? montantTVA7 = null,
        decimal? baseHT13 = null,
        decimal? montantTVA13 = null,
        decimal? baseHT19 = null,
        decimal? montantTVA19 = null)
    {
        Date = date;
        Description = description ?? string.Empty;
        MontantTotal = montantTotal;
        BaseHT0 = baseHT0 ?? 0;
        MontantTVA0 = montantTVA0 ?? 0;
        BaseHT7 = baseHT7 ?? 0;
        MontantTVA7 = montantTVA7 ?? 0;
        BaseHT13 = baseHT13 ?? 0;
        MontantTVA13 = montantTVA13 ?? 0;
        BaseHT19 = baseHT19 ?? 0;
        MontantTVA19 = montantTVA19 ?? 0;
        DocumentStoragePath = documentStoragePath;
    }

    public void Valider()
    {
        if (Statut != DocumentStatus.Draft)
        {
            throw new InvalidOperationException("Seul un document en brouillon peut être validé.");
        }
        Statut = DocumentStatus.Valid;
    }

    public int Id { get; private set; }

    public int Num { get; private set; }

    public int IdTiersDepenseFonctionnement { get; private set; }

    public DateTime Date { get; private set; }

    public string Description { get; private set; } = null!;

    public decimal MontantTotal { get; private set; }

    /// <summary>TVA 0% - Base HT</summary>
    public decimal BaseHT0 { get; private set; }

    /// <summary>TVA 0% - Montant TVA</summary>
    public decimal MontantTVA0 { get; private set; }

    /// <summary>TVA 7% - Base HT</summary>
    public decimal BaseHT7 { get; private set; }

    /// <summary>TVA 7% - Montant TVA</summary>
    public decimal MontantTVA7 { get; private set; }

    /// <summary>TVA 13% - Base HT</summary>
    public decimal BaseHT13 { get; private set; }

    /// <summary>TVA 13% - Montant TVA</summary>
    public decimal MontantTVA13 { get; private set; }

    /// <summary>TVA 19% - Base HT</summary>
    public decimal BaseHT19 { get; private set; }

    /// <summary>TVA 19% - Montant TVA</summary>
    public decimal MontantTVA19 { get; private set; }

    public int AccountingYearId { get; private set; }

    public DocumentStatus Statut { get; private set; }

    public string? DocumentStoragePath { get; private set; }

    public virtual TiersDepenseFonctionnement IdTiersDepenseFonctionnementNavigation { get; set; } = null!;

    public virtual AccountingYear AccountingYear { get; set; } = null!;

    public virtual ICollection<PaiementTiersDepenseFactureDepense> PaiementTiersDepenseFactureDepense { get; set; } = new List<PaiementTiersDepenseFactureDepense>();
}
