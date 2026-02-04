#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Facture de dépense (sans lignes, sans lien stock).
/// Description + MontantTotal uniquement.
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
        string? documentStoragePath = null)
    {
        return new FactureDepense
        {
            Num = num,
            IdTiersDepenseFonctionnement = idTiersDepenseFonctionnement,
            Date = date,
            Description = description ?? string.Empty,
            MontantTotal = montantTotal,
            AccountingYearId = accountingYearId,
            Statut = DocumentStatus.Draft,
            DocumentStoragePath = documentStoragePath
        };
    }

    public void Update(
        DateTime date,
        string description,
        decimal montantTotal,
        string? documentStoragePath)
    {
        Date = date;
        Description = description ?? string.Empty;
        MontantTotal = montantTotal;
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

    public int AccountingYearId { get; private set; }

    public DocumentStatus Statut { get; private set; }

    public string? DocumentStoragePath { get; private set; }

    public virtual TiersDepenseFonctionnement IdTiersDepenseFonctionnementNavigation { get; set; } = null!;

    public virtual AccountingYear AccountingYear { get; set; } = null!;

    public virtual ICollection<PaiementTiersDepenseFactureDepense> PaiementTiersDepenseFactureDepense { get; set; } = new List<PaiementTiersDepenseFactureDepense>();
}
