#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class Inventaire : IAccountingYearEntity
{
    private Inventaire()
    {
    }

    public static Inventaire CreateInventaire(
        int num,
        int accountingYearId,
        DateTime dateInventaire,
        string? description)
    {
        return new Inventaire
        {
            Num = num,
            AccountingYearId = accountingYearId,
            DateInventaire = dateInventaire,
            Description = description,
            Statut = InventaireStatut.Brouillon
        };
    }

    public void UpdateInventaire(
        DateTime dateInventaire,
        string? description)
    {
        DateInventaire = dateInventaire;
        Description = description;
    }

    public void Valider()
    {
        if (Statut != InventaireStatut.Brouillon)
        {
            throw new InvalidOperationException("Seul un inventaire en brouillon peut être validé.");
        }
        Statut = InventaireStatut.Valide;
    }

    public void Cloturer()
    {
        if (Statut != InventaireStatut.Valide)
        {
            throw new InvalidOperationException("Seul un inventaire validé peut être clôturé.");
        }
        Statut = InventaireStatut.Cloture;
    }

    public int Id { get; private set; }

    public int Num { get; private set; }

    public int AccountingYearId { get; private set; }

    public DateTime DateInventaire { get; private set; }

    public string? Description { get; private set; }

    public InventaireStatut Statut { get; private set; }

    public virtual AccountingYear AccountingYear { get; set; } = null!;

    public virtual ICollection<LigneInventaire> LigneInventaire { get; set; } = new List<LigneInventaire>();
}

