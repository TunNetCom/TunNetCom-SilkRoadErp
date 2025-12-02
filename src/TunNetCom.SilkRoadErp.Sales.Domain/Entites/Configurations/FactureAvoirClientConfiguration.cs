using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public partial class FactureAvoirClientConfiguration : IEntityTypeConfiguration<FactureAvoirClient>
{
    public void Configure(EntityTypeBuilder<FactureAvoirClient> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK_dbo.FactureAvoirClient");
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.HasIndex(e => e.Num)
            .IsUnique()
            .HasDatabaseName("IX_FactureAvoirClient_Num");

        entity.Property(e => e.Date)
            .HasColumnType("datetime")
            .HasColumnName("date");
        entity.Property(e => e.IdClient).HasColumnName("id_client");
        entity.Property(e => e.NumFactureAvoirClientSurPage).HasColumnName("Num_FactureAvoirClientSurPage");
        entity.Property(e => e.NumFacture).HasColumnName("Num_Facture");
        entity.Property(e => e.Statut)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        entity.ToTable("FactureAvoirClient");

        entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.FactureAvoirClient)
            .HasForeignKey(d => d.IdClient)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_dbo.FactureAvoirClient_dbo.Client_id_client");

        entity.HasOne(d => d.NumFactureNavigation).WithMany(p => p.FactureAvoirClient)
            .HasForeignKey(d => d.NumFacture)
            .HasPrincipalKey(p => p.Num)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_dbo.FactureAvoirClient_dbo.Facture_Num_Facture");

        entity.HasOne(d => d.AccountingYear).WithMany(p => p.FactureAvoirClient)
            .HasForeignKey(d => d.AccountingYearId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_dbo.FactureAvoirClient_dbo.AccountingYear_AccountingYearId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<FactureAvoirClient> entity);
}

