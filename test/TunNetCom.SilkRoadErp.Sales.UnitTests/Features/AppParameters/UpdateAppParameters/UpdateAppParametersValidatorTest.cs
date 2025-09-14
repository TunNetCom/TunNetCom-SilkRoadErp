
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.UpdateAppParameters;
public class UpdateAppParametersValidatorTest
{
    private readonly UpdateAppParametersValidator _validator;
    public UpdateAppParametersValidatorTest()
    {
        _validator = new UpdateAppParametersValidator();
    }
    [Fact]
    public void Should_Have_Error_When_NomSociete_Is_Empty()
    {
        var command = new UpdateAppParametersCommand(
            NomSociete: "",
            Timbre: 0,
            Adresse: "Valid Adresse",
            Tel: "0123456789",
            Fax: null,
            Email: null,
            MatriculeFiscale: null,
            CodeTva: null,
            CodeCategorie: null,
            EtbSecondaire: null,
            PourcentageFodec: 0,
            AdresseRetenu: null,
            PourcentageRetenu: 0);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.NomSociete)
              .WithErrorMessage("app_name_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_NomSociete_Too_Long()
    {
        var command = new UpdateAppParametersCommand(
            NomSociete: new string('A', 51), 
            Timbre: 0,
            Adresse: "Valid Adresse",
            Tel: "0123456789",
            Fax: null,
            Email: null,
            MatriculeFiscale: null,
            CodeTva: null,
            CodeCategorie: null,
            EtbSecondaire: null,
            PourcentageFodec: 0,
            AdresseRetenu: null,
            PourcentageRetenu: 0);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.NomSociete)
              .WithErrorMessage("app_name_must_be_less_than_50_characters");
    }

    [Fact]
    public void Should_Have_Error_When_Tel_Is_Empty()
    {
        var command = new UpdateAppParametersCommand(
            NomSociete: "Valid Name",
            Timbre: 0,
            Adresse: "Valid Adresse",
            Tel: "",
            Fax: null,
            Email: null,
            MatriculeFiscale: null,
            CodeTva: null,
            CodeCategorie: null,
            EtbSecondaire: null,
            PourcentageFodec: 0,
            AdresseRetenu: null,
            PourcentageRetenu: 0);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Tel)
              .WithErrorMessage("tel_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_Adresse_Is_Empty()
    {
        var command = new UpdateAppParametersCommand(
            NomSociete: "Valid Name",
            Timbre: 0,
            Adresse: "",
            Tel: "0123456789",
            Fax: null,
            Email: null,
            MatriculeFiscale: null,
            CodeTva: null,
            CodeCategorie: null,
            EtbSecondaire: null,
            PourcentageFodec: 0,
            AdresseRetenu: null,
            PourcentageRetenu: 0);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Adresse)
              .WithErrorMessage("adresse_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = new UpdateAppParametersCommand(
            NomSociete: "Valid Name",
            Timbre: 0,
            Adresse: "Valid Adresse",
            Tel: "0123456789",
            Fax: null,
            Email: "invalid-email",
            MatriculeFiscale: null,
            CodeTva: null,
            CodeCategorie: null,
            EtbSecondaire: null,
            PourcentageFodec: 0,
            AdresseRetenu: null,
            PourcentageRetenu: 0);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("email_must_be_a_valid_email_address");
    }

    [Fact]
    public void Should_Have_Error_When_Adresse_Too_Long()
    {
        var command = new UpdateAppParametersCommand(
            NomSociete: "Valid Name",
            Timbre: 0,
            Adresse: new string('A', 51),
            Tel: "0123456789",
            Fax: null,
            Email: null,
            MatriculeFiscale: null,
            CodeTva: null,
            CodeCategorie: null,
            EtbSecondaire: null,
            PourcentageFodec: 0,
            AdresseRetenu: null,
            PourcentageRetenu: 0);

        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Adresse)
              .WithErrorMessage("adresse_must_be_less_than_50_characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Data_Is_Valid()
    {
        var command = new UpdateAppParametersCommand(
            NomSociete: "Valid Name",
            Timbre: 0.5m,
            Adresse: "Valid Adresse",
            Tel: "0123456789",
            Fax: "123456",
            Email: "email@example.com",
            MatriculeFiscale: "MF123",
            CodeTva: "TVA001",
            CodeCategorie: "CAT01",
            EtbSecondaire: "ETB01",
            PourcentageFodec: 1.5m,
            AdresseRetenu: "Retenu Address",
            PourcentageRetenu: 1.0);

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
