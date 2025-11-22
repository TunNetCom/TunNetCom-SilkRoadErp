using FluentValidation.Results;

public class UpdateProviderValidatorTest
{
    private readonly UpdateProviderValidator _validator;

    public UpdateProviderValidatorTest()
    {
        _validator = new UpdateProviderValidator();
    }

    [Fact]
    public void Validator_Should_Pass_For_Valid_Command()
    {
        // Arrange
        var command = new UpdateProviderCommand(
            Id: 1,
            Nom: "Valid Name",
            Tel: "+12345678901",
            Fax: "123456",
            Matricule: "MAT123",
            Code: "CODE123",
            CodeCat: "CAT123",
            EtbSec: "ETB123",
            Mail: "test@example.com",
            MailDeux: "test2@example.com",
            Constructeur: true,
            Adresse: "Valid Address"
        );

        // Act
        ValidationResult result = _validator.Validate(command);

        // Assert
        _ = result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "name_is_required")]
    [InlineData(null, "name_is_required")]
    [InlineData("ThisNameIsWayTooLongAndShouldFailBecauseItExceedsFiftyCharactersLimit", "name_must_be_less_than_50_characters")]
    public void Validator_Should_Fail_For_Invalid_Name(string name, string expectedErrorMessage)
    {
        // Arrange
        var command = new UpdateProviderCommand(
            Id: 1,
            Nom: name,
            Tel: "+12345678901",
            Fax: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: "valid@mail.com",
            MailDeux: "valid2@mail.com",
            Constructeur: true,
            Adresse: "Address"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        _ = result.IsValid.Should().BeFalse();
        _ = result.Errors.Should().Contain(e => e.ErrorMessage == expectedErrorMessage);
    }

    [Theory]
    [InlineData("", "mobile_number_is_required")]
    [InlineData(null, "mobile_number_is_required")]
    [InlineData("123", "mobile_number_must_be_heigher_than_10_and_less_than_15_numbers")]
    [InlineData("+1234567890123456", "mobile_number_must_be_heigher_than_10_and_less_than_15_numbers")]
    public void Validator_Should_Fail_For_Invalid_Tel(string tel, string expectedErrorMessage)
    {
        // Arrange
        var command = new UpdateProviderCommand(
            Id: 1,
            Nom: "Valid Name",
            Tel: tel,
            Fax: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: "valid@mail.com",
            MailDeux: "valid2@mail.com",
            Constructeur: true,
            Adresse: "Address"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        _ = result.IsValid.Should().BeFalse();
        _ = result.Errors.Should().Contain(e => e.ErrorMessage == expectedErrorMessage);
    }

    [Fact]
    public void Validator_Should_Fail_For_Adresse_Too_Long()
    {
        // Arrange
        var longAddress = new string('a', 51);

        var command = new UpdateProviderCommand(
            Id: 1,
            Nom: "Valid Name",
            Tel: "+12345678901",
            Fax: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: "valid@mail.com",
            MailDeux: "valid2@mail.com",
            Constructeur: true,
            Adresse: longAddress
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        _ = result.IsValid.Should().BeFalse();
        _ = result.Errors.Should().Contain(e => e.ErrorMessage == "adress_must_be_less_than_50_characters");
    }
}