#nullable enable
using System;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class PaiementClientBonDeLivraison : ITenantEntity
{
    private PaiementClientBonDeLivraison()
    {
    }

    public static PaiementClientBonDeLivraison Create(int paiementClientId, int bonDeLivraisonId)
    {
        return new PaiementClientBonDeLivraison
        {
            PaiementClientId = paiementClientId,
            BonDeLivraisonId = bonDeLivraisonId
        };
    }

    public int PaiementClientId { get; private set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

    public int BonDeLivraisonId { get; private set; }

    public virtual PaiementClient PaiementClient { get; set; } = null!;

    public virtual BonDeLivraison BonDeLivraison { get; set; } = null!;
}

