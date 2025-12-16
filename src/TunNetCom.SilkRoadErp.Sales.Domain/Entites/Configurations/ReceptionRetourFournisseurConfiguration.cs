using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations
{
    public partial class ReceptionRetourFournisseurConfiguration : IEntityTypeConfiguration<ReceptionRetourFournisseur>
    {
        public void Configure(EntityTypeBuilder<ReceptionRetourFournisseur> entity)
        {
            entity.ToTable("ReceptionRetourFournisseur");

            entity.HasKey(e => e.Id).HasName("PK_dbo.ReceptionRetourFournisseur");
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.RetourMarchandiseFournisseurId)
                .HasColumnName("RetourMarchandiseFournisseurId")
                .IsRequired();

            entity.Property(e => e.DateReception)
                .HasColumnType("datetime")
                .HasColumnName("date_reception")
                .IsRequired();

            entity.Property(e => e.Utilisateur)
                .HasMaxLength(256)
                .HasColumnName("utilisateur")
                .IsRequired();

            entity.Property(e => e.Commentaire)
                .HasMaxLength(1000)
                .HasColumnName("commentaire")
                .IsRequired(false);

            entity.HasIndex(e => e.RetourMarchandiseFournisseurId)
                .HasDatabaseName("IX_ReceptionRetourFournisseur_RetourMarchandiseFournisseurId");

            entity.HasOne(d => d.RetourMarchandiseFournisseurNavigation)
                .WithMany(p => p.ReceptionRetourFournisseur)
                .HasForeignKey(d => d.RetourMarchandiseFournisseurId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ReceptionRetourFournisseur_dbo.RetourMarchandiseFournisseur_RetourMarchandiseFournisseurId");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<ReceptionRetourFournisseur> entity);
    }
}
