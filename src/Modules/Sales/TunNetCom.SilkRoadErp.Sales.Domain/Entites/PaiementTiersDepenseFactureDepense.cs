#nullable enable
using System;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Liaison entre un paiement tiers dépense et une facture dépense (traçabilité).
/// </summary>
public class PaiementTiersDepenseFactureDepense : ITenantEntity
{
    private PaiementTiersDepenseFactureDepense()
    {
    }

    public static PaiementTiersDepenseFactureDepense Create(int paiementTiersDepenseId, int factureDepenseId)
    {
        return new PaiementTiersDepenseFactureDepense
        {
            PaiementTiersDepenseId = paiementTiersDepenseId,
            FactureDepenseId = factureDepenseId
        };
    }

    public int PaiementTiersDepenseId { get; private set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

    public int FactureDepenseId { get; private set; }

    public virtual PaiementTiersDepense PaiementTiersDepense { get; set; } = null!;

    public virtual FactureDepense FactureDepense { get; set; } = null!;
}
