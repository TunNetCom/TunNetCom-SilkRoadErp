#nullable enable
using System;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class RetenueSourceFournisseur : IAccountingYearEntity, ITenantEntity
{
    public int Id { get; set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

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


