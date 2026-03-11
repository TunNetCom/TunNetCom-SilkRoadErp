#nullable enable
using System;
using System.Collections.Generic;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class Banque : ITenantEntity
{
    private Banque()
    {
    }

    public static Banque CreateBanque(string nom)
    {
        return new Banque
        {
            Nom = nom
        };
    }

    public void UpdateBanque(string nom)
    {
        this.Nom = nom;
    }

    public int Id { get; private set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

    public string Nom { get; private set; } = null!;

    public virtual ICollection<PaiementClient> PaiementClient { get; set; } = new List<PaiementClient>();

    public virtual ICollection<PaiementFournisseur> PaiementFournisseur { get; set; } = new List<PaiementFournisseur>();

    public virtual ICollection<PaiementTiersDepense> PaiementTiersDepense { get; set; } = new List<PaiementTiersDepense>();

    public virtual ICollection<CompteBancaire> CompteBancaire { get; set; } = new List<CompteBancaire>();
}

