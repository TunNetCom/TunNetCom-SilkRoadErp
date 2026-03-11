using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class TejCertificatFactureDepenseConfiguration : IEntityTypeConfiguration<TejCertificatFactureDepense>
{
    public void Configure(EntityTypeBuilder<TejCertificatFactureDepense> entity)
    {
        entity.HasKey(e => e.FactureDepenseId)
            .HasName("PK_TejCertificatFactureDepense");

        entity.ToTable("TejCertificatFactureDepense");

        entity.Property(e => e.FactureDepenseId)
            .HasColumnName("FactureDepenseId");
        entity.Property(e => e.RefCertif)
            .HasMaxLength(50)
            .HasColumnName("RefCertif");

        entity.HasOne(e => e.FactureDepense)
            .WithMany()
            .HasForeignKey(e => e.FactureDepenseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
