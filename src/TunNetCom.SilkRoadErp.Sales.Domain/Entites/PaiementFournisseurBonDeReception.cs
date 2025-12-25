#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class PaiementFournisseurBonDeReception
{
    private PaiementFournisseurBonDeReception()
    {
    }

    public static PaiementFournisseurBonDeReception Create(int paiementFournisseurId, int bonDeReceptionId)
    {
        return new PaiementFournisseurBonDeReception
        {
            PaiementFournisseurId = paiementFournisseurId,
            BonDeReceptionId = bonDeReceptionId
        };
    }

    public int PaiementFournisseurId { get; private set; }

    public int BonDeReceptionId { get; private set; }

    public virtual PaiementFournisseur PaiementFournisseur { get; set; } = null!;

    public virtual BonDeReception BonDeReception { get; set; } = null!;
}

