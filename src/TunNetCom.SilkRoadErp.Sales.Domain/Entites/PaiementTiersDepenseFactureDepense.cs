#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Liaison entre un paiement tiers dépense et une facture dépense (traçabilité).
/// </summary>
public class PaiementTiersDepenseFactureDepense
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

    public int FactureDepenseId { get; private set; }

    public virtual PaiementTiersDepense PaiementTiersDepense { get; set; } = null!;

    public virtual FactureDepense FactureDepense { get; set; } = null!;
}
