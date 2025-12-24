#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class PaiementClientFacture
{
    private PaiementClientFacture()
    {
    }

    public static PaiementClientFacture Create(int paiementClientId, int factureId)
    {
        return new PaiementClientFacture
        {
            PaiementClientId = paiementClientId,
            FactureId = factureId
        };
    }

    public int PaiementClientId { get; private set; }

    public int FactureId { get; private set; }

    public virtual PaiementClient PaiementClient { get; set; } = null!;

    public virtual Facture Facture { get; set; } = null!;
}

