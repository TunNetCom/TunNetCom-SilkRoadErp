#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class FactureAvoirClient : IAccountingYearEntity
{
    private FactureAvoirClient()
    {
    }

    public static FactureAvoirClient CreateFactureAvoirClient(
        int numFactureAvoirClientSurPage,
        int idClient,
        DateTime date,
        int? numFacture,
        int accountingYearId)
    {
        return new FactureAvoirClient
        {
            NumFactureAvoirClientSurPage = numFactureAvoirClientSurPage,
            IdClient = idClient,
            Date = date,
            NumFacture = numFacture,
            AccountingYearId = accountingYearId,
            Statut = DocumentStatus.Draft
        };
    }

    public void Valider()
    {
        if (Statut != DocumentStatus.Draft)
        {
            throw new InvalidOperationException("Seul un document en brouillon peut être validé.");
        }
        Statut = DocumentStatus.Valid;
    }

    public void UpdateFactureAvoirClient(
        int numFactureAvoirClientSurPage,
        int idClient,
        DateTime date,
        int? numFacture,
        int accountingYearId)
    {
        this.NumFactureAvoirClientSurPage = numFactureAvoirClientSurPage;
        this.IdClient = idClient;
        this.Date = date;
        this.NumFacture = numFacture;
        this.AccountingYearId = accountingYearId;
    }

    public int Id { get; private set; }

    public int Num { get; private set; }

    public int NumFactureAvoirClientSurPage { get; private set; }

    public int IdClient { get; private set; }

    public DateTime Date { get; private set; }

    public int? NumFacture { get; private set; }

    public int AccountingYearId { get; private set; }

    public DocumentStatus Statut { get; private set; }

    public virtual Client IdClientNavigation { get; private set; } = null!;

    public virtual AccountingYear AccountingYear { get; private set; } = null!;

    public virtual ICollection<Avoirs> Avoirs { get; set; } = new List<Avoirs>();

    public virtual Facture? NumFactureNavigation { get; private set; }
}

