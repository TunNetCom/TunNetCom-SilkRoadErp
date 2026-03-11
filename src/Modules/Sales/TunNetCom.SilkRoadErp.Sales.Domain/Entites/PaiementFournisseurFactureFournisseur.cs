#nullable enable
using System;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class PaiementFournisseurFactureFournisseur : ITenantEntity
{
    private PaiementFournisseurFactureFournisseur()
    {
    }

    public static PaiementFournisseurFactureFournisseur Create(int paiementFournisseurId, int factureFournisseurId)
    {
        return new PaiementFournisseurFactureFournisseur
        {
            PaiementFournisseurId = paiementFournisseurId,
            FactureFournisseurId = factureFournisseurId
        };
    }

    public int PaiementFournisseurId { get; private set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

    public int FactureFournisseurId { get; private set; }

    public virtual PaiementFournisseur PaiementFournisseur { get; set; } = null!;

    public virtual FactureFournisseur FactureFournisseur { get; set; } = null!;
}

