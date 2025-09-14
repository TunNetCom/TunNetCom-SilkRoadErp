using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;
using Xunit;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers.UpdateCustomer;

public class UpdateCustomerValidatorTests
{
    private readonly UpdateCustomerValidator _validator;

    public UpdateCustomerValidatorTests()
    {
        _validator = new UpdateCustomerValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Nom_Is_Empty()
    {
        var command = new UpdateCustomerCommand(1, "", null, null, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Nom)
              .WithErrorMessage("nom_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_Nom_Exceeds_MaxLength()
    {
        var command = new UpdateCustomerCommand(1, new string('A', 51), null, null, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Nom)
              .WithErrorMessage("nom_must_be_less_than_50_characters");
    }

    [Fact]
    public void Should_Have_Error_When_Tel_Is_Empty()
    {
        var command = new UpdateCustomerCommand(1, "John", "", null, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Tel)
              .WithErrorMessage("tel_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_Tel_Is_Invalid()
    {
        var command = new UpdateCustomerCommand(1, "John", "123", null, null, null, null, null, null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Tel)
              .WithErrorMessage("tel_must_be_heigher_than_10_and_less_than_15");
    }

    [Fact]
    public void Should_Have_Error_When_Adresse_Exceeds_MaxLength()
    {
        var command = new UpdateCustomerCommand(1, "John", null, new string('A', 51), null, null, null, null, null);
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Adresse)
              .WithErrorMessage("adresse_must_be_less_than_50_characters");
    }

    [Fact]
    public void Should_Have_Error_When_Mail_Is_Invalid()
    {
        var command = new UpdateCustomerCommand(1, "John", null, null, null, null, null, null, "invalid-email");
        var result = _validator.TestValidate(command);
        _ = result.ShouldHaveValidationErrorFor(x => x.Mail)
              .WithErrorMessage("mail_must_be_a_valid_email_address");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Data_Is_Valid()
    {
        var command = new UpdateCustomerCommand(
            1,
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