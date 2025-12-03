#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class RetenueSourceFournisseur : IAccountingYearEntity
{
    public int Id { get; set; }

    public int NumFactureFournisseur { get; set; }

    public string? NumTej { get; set; }

    public decimal MontantAvantRetenu { get; set; }

    public double TauxRetenu { get; set; }

    public decimal MontantApresRetenu { get; set; }

    public string? PdfStoragePath { get; set; }

    public DateTime DateCreation { get; set; }

    public int AccountingYearId { get; set; }

    public virtual FactureFournisseur NumFactureFournisseurNavigation { get; set; } = null!;

    public virtual AccountingYear AccountingYear { get; set; } = null!;
}


