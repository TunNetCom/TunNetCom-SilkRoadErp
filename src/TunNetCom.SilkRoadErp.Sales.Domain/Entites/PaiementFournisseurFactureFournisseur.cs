#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class PaiementFournisseurFactureFournisseur
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

    public int FactureFournisseurId { get; private set; }

    public virtual PaiementFournisseur PaiementFournisseur { get; set; } = null!;

    public virtual FactureFournisseur FactureFournisseur { get; set; } = null!;
}

