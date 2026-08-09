using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class InventaireTest
{
    [Fact]
    public void CreateInventaire_ShouldSetProperties()
    {
        var date = new DateTime(2024, 12, 31);

        var inventaire = Inventaire.CreateInventaire(
            num: 4,
            accountingYearId: 2024,
            dateInventaire: date,
            description: "Inventaire de fin d'année");

        inventaire.Num.Should().Be(4);
        inventaire.AccountingYearId.Should().Be(2024);
        inventaire.DateInventaire.Should().Be(date);
        inventaire.Description.Should().Be("Inventaire de fin d'année");
        inventaire.Statut.Should().Be(InventaireStatut.Brouillon);
        inventaire.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        inventaire.LigneInventaire.Should().BeEmpty();
    }

    [Fact]
    public void UpdateInventaire_ShouldUpdateProperties()
    {
        var inventaire = Inventaire.CreateInventaire(
            num: 4,
            accountingYearId: 2024,
            dateInventaire: new DateTime(2024, 12, 31),
            description: "Old");
        var newDate = new DateTime(2025, 1, 1);

        inventaire.UpdateInventaire(dateInventaire: newDate, description: "New");

        inventaire.DateInventaire.Should().Be(newDate);
        inventaire.Description.Should().Be("New");
    }

    [Fact]
    public void Valider_WhenBrouillon_ShouldSetValide()
    {
        var inventaire = Inventaire.CreateInventaire(1, 2024, DateTime.Now, null);

        inventaire.Valider();

        inventaire.Statut.Should().Be(InventaireStatut.Valide);
    }

    [Fact]
    public void Valider_WhenNotBrouillon_ShouldThrow()
    {
        var inventaire = Inventaire.CreateInventaire(1, 2024, DateTime.Now, null);
        inventaire.Valider();

        var act = () => inventaire.Valider();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Seul un inventaire en brouillon peut être validé.");
    }

    [Fact]
    public void Cloturer_WhenValide_ShouldSetCloture()
    {
        var inventaire = Inventaire.CreateInventaire(1, 2024, DateTime.Now, null);
        inventaire.Valider();

        inventaire.Cloturer();

        inventaire.Statut.Should().Be(InventaireStatut.Cloture);
    }

    [Fact]
    public void Cloturer_WhenNotValide_ShouldThrow()
    {
        var inventaire = Inventaire.CreateInventaire(1, 2024, DateTime.Now, null);

        var act = () => inventaire.Cloturer();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Seul un inventaire validé peut être clôturé.");
    }
}
