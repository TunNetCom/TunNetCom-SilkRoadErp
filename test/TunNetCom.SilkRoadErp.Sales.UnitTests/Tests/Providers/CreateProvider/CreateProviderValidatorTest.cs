namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers
{
    public class CreateProviderValidatorTest
    {
        private readonly CreateProviderValidator _validator;
        public CreateProviderValidatorTest()
        {
            _validator = new CreateProviderValidator();
        }
        [Fact]
        public void Should_Pass_Validation_When_Request_Is_Valid()
        {
            // Arrange
            var command = new CreateProviderCommand(
                Nom: "Valid Name",
                Tel: "+21612345678",
                Fax: "12345",
                Matricule: "MAT123",
                Code: "CODE001",
                CodeCat: "CAT001",
                EtbSec: "SEC",
                Mail: "valid@mail.com",
                MailDeux: "second@mail.com",
                Constructeur: true,
                Adresse: "Somewhere"
            );
            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_Name_Is_Empty()
        {
            var command = ValidCommand() with { Nom = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Nom)
                .WithErrorMessage("name_is_required");
        }

        [Fact]
        public void Should_Fail_When_Tel_Is_Invalid()
        {
            var command = ValidCommand() with { Tel = "123" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Tel)
                .WithErrorMessage("mobile_number_must_be_heigher_than_10_and_less_than_15_numbers");
        }

        [Fact]
        public void Should_Fail_When_Mail_Is_Invalid()
        {
            var command = ValidCommand() with { Mail = "invalid-email" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Mail)
                .WithErrorMessage("mail_must_be_a_valid_email_address");
        }

        [Fact]
        public void Should_Fail_When_MailDeux_Is_Invalid()
        {
            var command = ValidCommand() with { MailDeux = "not-an-email" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.MailDeux)
                .WithErrorMessage("mail_must_be_a_valid_email_address");
        }

        [Fact]
        public void Should_Fail_When_Adresse_Too_Long()
        {
            var command = ValidCommand() with { Adresse = new string('a', 51) };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Adresse)
                .WithErrorMessage("adress_must_be_less_than_50_characters");
        }
        private CreateProviderCommand ValidCommand() => new(
            Nom: "Provider",
            Tel: "+21612345678",
            Fax: "Fax",
            Matricule: "Mat",
            Code: "Code",
            CodeCat: "Cat",
            EtbSec: "Etb",
            Mail: "email@domain.com",
            MailDeux: "email2@domain.com",
            Constructeur: true,
            Adresse: "Adresse"
        );
    }
}
