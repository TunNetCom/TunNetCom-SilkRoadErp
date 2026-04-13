using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes.CreateReceiptNote;

public class CreateReceiptNoteValidatorTest
{
    private readonly CreateReceiptNoteValidator _validator;

    public CreateReceiptNoteValidatorTest()
    {
        _validator = new CreateReceiptNoteValidator();
    }

    [Fact]
    public void Should_Have_Error_When_NumBonFournisseur_Is_Empty()
    {
        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 0,
            DateLivraison: DateTime.Today,
            IdFournisseur: 1,
            Date: DateTime.Today,
            NumFactureFournisseur: null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.NumBonFournisseur)
            .WithErrorMessage("provider_receipt_number_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_DateLivraison_Is_Default()
    {
        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 100,
            DateLivraison: default,
            IdFournisseur: 1,
            Date: DateTime.Today,
            NumFactureFournisseur: null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.DateLivraison)
            .WithErrorMessage("delivery_date_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_IdFournisseur_Is_Negative()
    {
        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 100,
            DateLivraison: DateTime.Today,
            IdFournisseur: -1,
            Date: DateTime.Today,
            NumFactureFournisseur: null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.IdFournisseur)
            .WithErrorMessage("providerid_must_be_greater_than_or_equal_to_0");
    }

    [Fact]
    public void Should_Have_Error_When_IdFournisseur_Is_Zero()
    {
        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 100,
            DateLivraison: DateTime.Today,
            IdFournisseur: 0,
            Date: DateTime.Today,
            NumFactureFournisseur: null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.IdFournisseur);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "providerid_is_required" || e.ErrorMessage == "providerid_must_be_greater_than_or_equal_to_0");
    }

    [Fact]
    public void Should_Have_Error_When_Date_Is_Default()
    {
        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 100,
            DateLivraison: DateTime.Today,
            IdFournisseur: 1,
            Date: default,
            NumFactureFournisseur: null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.Date)
            .WithErrorMessage("date_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_NumFactureFournisseur_Is_Negative()
    {
        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 100,
            DateLivraison: DateTime.Today,
            IdFournisseur: 1,
            Date: DateTime.Today,
            NumFactureFournisseur: -1);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.NumFactureFournisseur)
            .WithErrorMessage("invoice_number_must_be_greater_than_or_equal_to_0");
    }

    [Fact]
    public void Should_Not_Have_Errors_For_Valid_Command()
    {
        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 100,
            DateLivraison: DateTime.Today,
            IdFournisseur: 1,
            Date: DateTime.Today,
            NumFactureFournisseur: 10);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Errors_For_Valid_Command_With_Null_NumFactureFournisseur()
    {
        var command = new CreateReceiptNoteCommand(
            NumBonFournisseur: 100,
            DateLivraison: DateTime.Today,
            IdFournisseur: 1,
            Date: DateTime.Today,
            NumFactureFournisseur: null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
