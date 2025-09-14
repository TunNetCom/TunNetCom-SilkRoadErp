public class CreateProviderValidatorTest
{
    private readonly CreateProviderValidator _validator;
    public CreateProviderValidatorTest()
    {
        _validator = new CreateProviderValidator();
    }
    [Fact]
    public void Should_Have_Error_When_Nom_Is_Null_Or_Empty()
    {
        _ = _validator.TestValidate(new CreateProviderCommand(
            Nom: "",
            Tel: "+12345678901",
            Fax: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: null,
            MailDeux: null,
            Constructeur: true,
            Adresse: null))
            .ShouldHaveValidationErrorFor(x => x.Nom)
            .WithErrorMessage("name_is_required");
    }

    [Fact]
    public void Should_Have_Error_When_Nom_Too_Long()
    {
        var longName = new string('a', 51);
        _ = _validator.TestValidate(new CreateProviderCommand(
            Nom: longName,
            Tel: "+12345678901",
            Fax: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: null,
            MailDeux: null,
            Constructeur: true,
            Adresse: null))
            .ShouldHaveValidationErrorFor(x => x.Nom)
            .WithErrorMessage("name_must_be_less_than_50_characters");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Have_Error_When_Tel_Is_Null_Or_Empty(string tel)
    {
        _ = _validator.TestValidate(new CreateProviderCommand(
            Nom: "ValidName",
            Tel: tel,
            Fax: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: null,
            MailDeux: null,
            Constructeur: true,
            Adresse: null))
            .ShouldHaveValidationErrorFor(x => x.Tel)
            .WithErrorMessage("mobile_number_is_required");
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("abc1234567")]
    [InlineData("+123456789")]
    public void Should_Have_Error_When_Tel_Is_Invalid(string tel)
    {
        _ = _validator.TestValidate(new CreateProviderCommand(
            Nom: "ValidName",
            Tel: tel,
            Fax: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: null,
            MailDeux: null,
            Constructeur: true,
            Adresse: null))
            .ShouldHaveValidationErrorFor(x => x.Tel)
            .WithErrorMessage("mobile_number_must_be_heigher_than_10_and_less_than_15_numbers");
    }

    [Fact]
    public void Should_Have_Error_When_Mail_Is_Invalid()
    {
        _ = _validator.TestValidate(new CreateProviderCommand(
            Nom: "ValidName",
            Tel: "+12345678901",
            Fax: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: "invalid-email",
            MailDeux: "invalid-email",
            Constructeur: true,
            Adresse: null))
            .ShouldHaveValidationErrorFor(x => x.Mail)
            .WithErrorMessage("mail_must_be_a_valid_email_address");

        _ = _validator.TestValidate(new CreateProviderCommand(
            Nom: "ValidName",
            Tel: "+12345678901",
            Fax: null,
            Matricule: null,
            Code: null,
            CodeCat: null,
            EtbSec: null,
            Mail: "valid@mail.com",
            MailDeux: "invalid-email",
            Constructeur: true,
            Adresse: null))
            .ShouldHaveValidationErrorFor(x => x.MailDeux)
            .WithErrorMessage("mail_must_be_a_valid_email_address");
    }

    [Fact]
    public void Should_Pass_For_Valid_Command()
    {
        var command = new CreateProviderCommand(
            Nom: "ValidName",
            Tel: "+12345678901",
            Fax: "12345",
            Matricule: "matricule",
            Code: "code",
            CodeCat: "codecat",
            EtbSec: "etbsec",
            Mail: "valid@mail.com",
            MailDeux: "valid2@mail.com",
            Constructeur: true,
            Adresse: "some address");

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
