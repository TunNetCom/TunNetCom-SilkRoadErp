using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;
public class CreateDeliveryNoteValidatorTest
{
    private readonly CreateDeliveryNoteValidator _validator;
    public CreateDeliveryNoteValidatorTest()
    {
        _validator = new CreateDeliveryNoteValidator();
    }

    [Fact]
    public void Should_Pass_For_Valid_Command()
    {
        var command = new CreateDeliveryNoteCommand(
            Date: DateTime.Today,
            TotHTva: 100m,
            TotTva: 20m,
            NetPayer: 120m,
            TempBl: TimeOnly.FromDateTime(DateTime.Now),
            NumFacture: 1,
            ClientId: 1,
            DeliveryNoteDetails: Array.Empty<LigneBlSubCommand>()
        );
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_Date_Is_Default()
    {
        var command = new CreateDeliveryNoteCommand(
            Date: default,
            TotHTva: 100m,
            TotTva: 20m,
            NetPayer: 120m,
            TempBl: TimeOnly.FromDateTime(DateTime.Now),
            NumFacture: 1,
            ClientId: 1,
            DeliveryNoteDetails: Array.Empty<LigneBlSubCommand>()
        );
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.Date)
            .WithErrorMessage("date_is_required");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Should_Have_Error_When_TotHTva_Is_Negative(decimal totHTva)
    {
        var command = new CreateDeliveryNoteCommand(
            Date: DateTime.Today,
            TotHTva: totHTva,
            TotTva: 20m,
            NetPayer: 120m,
            TempBl: TimeOnly.FromDateTime(DateTime.Now),
            NumFacture: 1,
            ClientId: 1,
            DeliveryNoteDetails: Array.Empty<LigneBlSubCommand>()
        );
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.TotHTva)
            .WithErrorMessage("tothtva_must_be_greater_than_or_equal_to_0");
    }

    [Fact]
    public void Should_Have_Error_When_TempBl_Is_Default()
    {
        var command = new CreateDeliveryNoteCommand(
            Date: DateTime.Today,
            TotHTva: 100m,
            TotTva: 20m,
            NetPayer: 120m,
            TempBl: default,
            NumFacture: 1,
            ClientId: 1,
            DeliveryNoteDetails: Array.Empty<LigneBlSubCommand>()
        );
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.TempBl)
            .WithErrorMessage("tempbl_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_NumFacture_Is_Negative()
    {
        var command = new CreateDeliveryNoteCommand(
            Date: DateTime.Today,
            TotHTva: 100m,
            TotTva: 20m,
            NetPayer: 120m,
            TempBl: TimeOnly.FromDateTime(DateTime.Now),
            NumFacture: -5,
            ClientId: 1,
            DeliveryNoteDetails: Array.Empty<LigneBlSubCommand>()
        );
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.NumFacture)
            .WithErrorMessage("numfacture_must_be_greater_than_or_equal_to_0");
    }

    [Fact]
    public void Should_Have_Error_When_ClientId_Is_Negative()
    {
        var command = new CreateDeliveryNoteCommand(
            Date: DateTime.Today,
            TotHTva: 100m,
            TotTva: 20m,
            NetPayer: 120m,
            TempBl: TimeOnly.FromDateTime(DateTime.Now),
            NumFacture: 1,
            ClientId: -3,
            DeliveryNoteDetails: Array.Empty<LigneBlSubCommand>()
        );
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.ClientId)
            .WithErrorMessage("clientid_must_be_greater_than_or_equal_to_0");
    }
}
