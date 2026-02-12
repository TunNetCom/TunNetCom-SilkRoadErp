using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class CompteBancaireConfiguration : IEntityTypeConfiguration<CompteBancaire>
{
    public void Configure(EntityTypeBuilder<CompteBancaire> entity)
    {
        entity.ToTable("CompteBancaire");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.BanqueId)
            .HasColumnName("BanqueId")
            .IsRequired();

        entity.Property(e => e.CodeEtablissement)
            .HasColumnName("CodeEtablissement")
            .HasMaxLength(10)
            .IsRequired();

        entity.Property(e => e.CodeAgence)
            .HasColumnName("CodeAgence")
            .HasMaxLength(10)
            .IsRequired();

        entity.Property(e => e.NumeroCompte)
            .HasColumnName("NumeroCompte")
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(e => e.CleRib)
            .HasColumnName("CleRib")
            .HasMaxLength(5)
            .IsRequired();

        entity.Property(e => e.Libelle)
            .HasColumnName("Libelle")
            .HasMaxLength(200);

        entity.HasOne(d => d.Banque)
            .WithMany(p => p.CompteBancaire)
            .HasForeignKey(d => d.BanqueId)
            .OnDelete(DeleteBehavior.Restrict);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<CompteBancaire> entity);
}
