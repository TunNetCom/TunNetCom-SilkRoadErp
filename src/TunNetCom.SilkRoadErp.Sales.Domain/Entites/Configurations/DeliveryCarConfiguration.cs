using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class DeliveryCarConfiguration : IEntityTypeConfiguration<DeliveryCar>
{
    public void Configure(EntityTypeBuilder<DeliveryCar> entity)
    {
        entity.ToTable("DeliveryCar");

        entity.HasKey(e => e.Id).HasName("PK_dbo.DeliveryCar");

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Matricule)
            .HasColumnName("Matricule")
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(e => e.Owner)
            .HasColumnName("Owner")
            .HasMaxLength(100)
            .IsRequired();

        entity.HasIndex(e => e.Matricule)
            .IsUnique()
            .HasDatabaseName("IX_DeliveryCar_Matricule");

        entity.HasMany(d => d.BonDeLivraisons)
            .WithOne(p => p.DeliveryCar)
            .HasForeignKey(d => d.DeliveryCarId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_dbo.BonDeLivraison_dbo.DeliveryCar_DeliveryCarId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<DeliveryCar> entity);
}

