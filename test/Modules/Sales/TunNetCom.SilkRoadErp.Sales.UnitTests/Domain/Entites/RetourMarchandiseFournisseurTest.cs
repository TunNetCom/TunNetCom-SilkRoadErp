using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class RetourMarchandiseFournisseurTest
{
    private static RetourMarchandiseFournisseur CreateRetour()
    {
        return RetourMarchandiseFournisseur.CreateRetourMarchandiseFournisseur(
            num: 1,
            date: new DateTime(2024, 7, 1),
            idFournisseur: 2,
            accountingYearId: 2024,
            totHTva: 900m,
            totTva: 171m,
            netPayer: 1071m);
    }

    private static LigneRetourMarchandiseFournisseur CreateLigne(int qteLi, int qteRecue)
    {
        return LigneRetourMarchandiseFournisseur.CreateRetourLine(
            retourMarchandiseFournisseurId: 1,
            productRef: "PRD-001",
            designationLigne: "Produit",
            quantity: qteLi,
            unitPrice: 100m,
            discount: 0,
            tax: 19,
            qteRecue: qteRecue);
    }

    [Fact]
    public void CreateRetourMarchandiseFournisseur_ShouldSetProperties()
    {
        var retour = CreateRetour();

        retour.Num.Should().Be(1);
        retour.Date.Should().Be(new DateTime(2024, 7, 1));
        retour.IdFournisseur.Should().Be(2);
        retour.AccountingYearId.Should().Be(2024);
        retour.TotHTva.Should().Be(900m);
        retour.TotTva.Should().Be(171m);
        retour.NetPayer.Should().Be(1071m);
        retour.StatutRetour.Should().Be(RetourFournisseurStatus.Draft);
        retour.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void UpdateRetourMarchandiseFournisseur_ShouldUpdateProperties()
    {
        var retour = CreateRetour();

        retour.UpdateRetourMarchandiseFournisseur(
            num: 2,
            date: new DateTime(2024, 8, 1),
            idFournisseur: 3,
            accountingYearId: 2025,
            totHTva: 800m,
            totTva: 152m,
            netPayer: 952m);

        retour.Num.Should().Be(2);
        retour.Date.Should().Be(new DateTime(2024, 8, 1));
        retour.IdFournisseur.Should().Be(3);
        retour.AccountingYearId.Should().Be(2025);
        retour.TotHTva.Should().Be(800m);
        retour.TotTva.Should().Be(152m);
        retour.NetPayer.Should().Be(952m);
    }

    [Fact]
    public void Valider_WhenDraft_ShouldSetValid()
    {
        var retour = CreateRetour();

        retour.Valider();

        retour.StatutRetour.Should().Be(RetourFournisseurStatus.Valid);
    }

    [Fact]
    public void Valider_WhenNotDraft_ShouldThrow()
    {
        var retour = CreateRetour();
        retour.Valider();

        var act = () => retour.Valider();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Seul un document en brouillon peut être validé.");
    }

    [Fact]
    public void PasserEnReparation_WhenValid_ShouldSetEnReparation()
    {
        var retour = CreateRetour();
        retour.Valider();

        retour.PasserEnReparation();

        retour.StatutRetour.Should().Be(RetourFournisseurStatus.EnReparation);
    }

    [Fact]
    public void PasserEnReparation_WhenNotValid_ShouldThrow()
    {
        var retour = CreateRetour();

        var act = () => retour.PasserEnReparation();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Seul un retour validé peut passer en réparation.");
    }

    [Fact]
    public void ValiderReception_WhenAllLinesComplete_ShouldSetCloture()
    {
        var retour = CreateRetour();
        retour.Valider();
        retour.LigneRetourMarchandiseFournisseur.Add(CreateLigne(qteLi: 10, qteRecue: 10));
        retour.LigneRetourMarchandiseFournisseur.Add(CreateLigne(qteLi: 5, qteRecue: 5));

        retour.ValiderReception();

        retour.StatutRetour.Should().Be(RetourFournisseurStatus.Cloture);
    }

    [Fact]
    public void ValiderReception_WhenPartialReception_ShouldSetReceptionPartielle()
    {
        var retour = CreateRetour();
        retour.Valider();
        retour.PasserEnReparation();
        retour.LigneRetourMarchandiseFournisseur.Add(CreateLigne(qteLi: 10, qteRecue: 4));

        retour.ValiderReception();

        retour.StatutRetour.Should().Be(RetourFournisseurStatus.ReceptionPartielle);
    }

    [Fact]
    public void ValiderReception_WhenNoLineReceived_ShouldSetEnReparation()
    {
        var retour = CreateRetour();
        retour.Valider();
        retour.PasserEnReparation();
        retour.LigneRetourMarchandiseFournisseur.Add(CreateLigne(qteLi: 10, qteRecue: 0));

        retour.ValiderReception();

        retour.StatutRetour.Should().Be(RetourFournisseurStatus.EnReparation);
    }

    [Fact]
    public void ValiderReception_WhenDraft_ShouldThrow()
    {
        var retour = CreateRetour();

        var act = () => retour.ValiderReception();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*retour validé, en réparation ou en réception partielle*");
    }

    [Fact]
    public void GetQuantiteEnAttenteReception_ShouldSumRemaining()
    {
        var retour = CreateRetour();
        retour.LigneRetourMarchandiseFournisseur.Add(CreateLigne(qteLi: 10, qteRecue: 4));
        retour.LigneRetourMarchandiseFournisseur.Add(CreateLigne(qteLi: 5, qteRecue: 5));
        retour.LigneRetourMarchandiseFournisseur.Add(CreateLigne(qteLi: 3, qteRecue: 0));

        retour.GetQuantiteEnAttenteReception().Should().Be(6 + 0 + 3);
    }

    [Fact]
    public void GetQuantiteTotaleRecue_ShouldSumReceived()
    {
        var retour = CreateRetour();
        retour.LigneRetourMarchandiseFournisseur.Add(CreateLigne(qteLi: 10, qteRecue: 4));
        retour.LigneRetourMarchandiseFournisseur.Add(CreateLigne(qteLi: 5, qteRecue: 5));

        retour.GetQuantiteTotaleRecue().Should().Be(9);
    }

    [Fact]
    public void Statut_ShouldMapToDocumentStatus()
    {
        var retour = CreateRetour();

        retour.Statut.Should().Be(DocumentStatus.Draft);
        retour.Valider();
        retour.Statut.Should().Be(DocumentStatus.Valid);
        retour.PasserEnReparation();
        retour.Statut.Should().Be(DocumentStatus.Valid);
    }
}
