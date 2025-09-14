using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;
public class UpdateCustomerValidatorTest
{
    private readonly UpdateCustomerValidator _validator;
    public UpdateCustomerValidatorTest()
    {
        _validator = new UpdateCustomerValidator();
    }
    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new UpdateCustomerCommand(
            Id: 1,
            Nom: "John Doe",
            Tel: "+21612345678",
            Adresse: "123 Main Street",
            Matricule: "M123456",
            Code: "C123",
            CodeCat: "CC123",
            EtbSec: "ETB",
            Mail: "john.doe@mail.com"
        );
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Nom_Is_Empty()
    {
        var command = new UpdateCustomerCommand(
            Id: 1,
            Nom: "",
            Tel: "+21612345678",
            Adresse: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: null
        );
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.Nom)
              .WithErrorMessage("nom_is_required");
    }

    [Fact]
    public void Should_Fail_When_Tel_Is_Invalid()
    {
        var command = new UpdateCustomerCommand(
            Id: 1,
            Nom: "Test",
            Tel: "123",
            Adresse: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: null
        );
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.Tel)
              .WithErrorMessage("tel_must_be_heigher_than_10_and_less_than_15");
    }

    [Fact]
    public void Should_Fail_When_Mail_Is_Invalid()
    {
        var command = new UpdateCustomerCommand(
            Id: 1,
            Nom: "Test",
            Tel: "+21612345678",
            Adresse: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: "invalid-email"
        );
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.Mail)
              .WithErrorMessage("mail_must_be_a_valid_email_address");
    }

    [Fact]
    public void Should_Fail_When_Field_Exceeds_MaxLength()
    {
        var longText = new string('a', 51);
        var command = new UpdateCustomerCommand(
            Id: 1,
            Nom: longText,
            Tel: "+21612345678",
            Adresse: longText,
            Matricule: longText,
            Code: longText,
            CodeCat: longText,
            EtbSec: longText,
            Mail: "john.doe@mail.com"
        );
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(c => c.Nom)
              .WithErrorMessage("nom_must_be_less_than_50_characters");
        _ = result.ShouldHaveValidationErrorFor(c => c.Adresse)
              .WithErrorMessage("adresse_must_be_less_than_50_characters");
        _ = result.ShouldHaveValidationErrorFor(c => c.Matricule)
              .WithErrorMessage("matricule_must_be_less_than_50_characters");
        _ = result.ShouldHaveValidationErrorFor(c => c.Code)
              .WithErrorMessage("code_must_be_less_than_50_characters");
        _ = result.ShouldHaveValidationErrorFor(c => c.CodeCat)
              .WithErrorMessage("codeCat_must_be_less_than_50_characters");
        _ = result.ShouldHaveValidationErrorFor(c => c.EtbSec)
              .WithErrorMessage("etbSec_must_be_less_than_50_characters");
    }
}
