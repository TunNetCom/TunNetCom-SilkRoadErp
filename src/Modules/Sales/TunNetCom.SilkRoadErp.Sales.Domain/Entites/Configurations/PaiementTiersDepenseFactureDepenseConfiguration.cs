using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class PaiementTiersDepenseFactureDepenseConfiguration : IEntityTypeConfiguration<PaiementTiersDepenseFactureDepense>
{
    public void Configure(EntityTypeBuilder<PaiementTiersDepenseFactureDepense> entity)
    {
        entity.ToTable("PaiementTiersDepenseFactureDepense");

        entity.HasKey(e => new { e.PaiementTiersDepenseId, e.FactureDepenseId });

        entity.Property(e => e.PaiementTiersDepenseId)
            .IsRequired();

        entity.Property(e => e.FactureDepenseId)
            .IsRequired();

        entity.HasOne(d => d.PaiementTiersDepense)
            .WithMany(p => p.FactureDepenses)
            .HasForeignKey(d => d.PaiementTiersDepenseId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PaiementTiersDepenseFactureDepense_PaiementTiersDepense");

        entity.HasOne(d => d.FactureDepense)
            .WithMany(p => p.PaiementTiersDepenseFactureDepense)
            .HasForeignKey(d => d.FactureDepenseId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaiementTiersDepenseFactureDepense_FactureDepense");
    }
}
