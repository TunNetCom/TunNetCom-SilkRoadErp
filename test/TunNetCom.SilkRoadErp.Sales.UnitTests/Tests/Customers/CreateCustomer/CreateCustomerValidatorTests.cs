using Xunit;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers.CreateCustomer;

public class CreateCustomerValidatorTest
{
    private readonly CreateCustomerValidator _validator;

    public CreateCustomerValidatorTest()
    {
        _validator = new CreateCustomerValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Nom_Is_Empty()
    {
        var command = new CreateCustomerCommand("", null, null, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Nom)
              .WithErrorMessage("nom_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_Nom_Exceeds_MaxLength()
    {
        var command = new CreateCustomerCommand(new string('A', 51), null, null, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Nom)
              .WithErrorMessage("nom_must_be_less_than_50_characters");
    }

    [Fact]
    public void Should_Have_Error_When_Tel_Is_Empty()
    {
        var command = new CreateCustomerCommand("John", "", null, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Tel)
              .WithErrorMessage("tel_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_Tel_Is_Invalid()
    {
        var command = new CreateCustomerCommand("John", "123", null, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Tel)
              .WithErrorMessage("tel_must_be_heigher_than_10_and_less_than_15");
    }

    [Fact]
    public void Should_Have_Error_When_Adresse_Exceeds_MaxLength()
    {
        var command = new CreateCustomerCommand("John", null, new string('A', 51), null, null, null, null, null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Adresse)
              .WithErrorMessage("adresse_must_be_less_than_50_characters");
    }

    [Fact]
    public void Should_Have_Error_When_Mail_Is_Invalid()
    {
        var command = new CreateCustomerCommand("John", null, null, null, null, null, null, "invalid-email");
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Mail)
              .WithErrorMessage("mail_must_be_a_valid_email_address");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Data_Is_Valid()
    {
        var command = new CreateCustomerCommand(
            "John Doe",
            "+123456789012",
            "123 Street",
            "MAT123",
            "C001",
            "CAT01",
            "ETB1",
            "john.doe@example.com"
        );

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
