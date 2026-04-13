namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Customers.CreateCustomer
{
    public class CreateCustomerValidatorTests
    {
        private readonly CreateCustomerValidator _validator;
        public CreateCustomerValidatorTests()
        {
            _validator = new CreateCustomerValidator();
        }
        [Fact]
        public void Should_Have_Error_When_Nom_Is_Empty()
        {
            var model = new CreateCustomerCommand(
                Nom: "",
                Tel: "+12345678901",
                Adresse: null,
                Matricule: null,
                Code: null,
                CodeCat: null,
                EtbSec: null,
                Mail: null);

            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.Nom)
                  .WithErrorMessage("nom_is_required");
        }

        [Fact]
        public void Should_Have_Error_When_Nom_Too_Long()
        {
            var longName = new string('a', 51);
            var model = new CreateCustomerCommand(
                Nom: longName,
                Tel: "+12345678901",
                Adresse: null,
                Matricule: null,
                Code: null,
                CodeCat: null,
                EtbSec: null,
                Mail: null);
            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.Nom)
                  .WithErrorMessage("nom_must_be_less_than_50_characters");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Tel_Is_Null_Or_Empty(string tel)
        {
            var model = new CreateCustomerCommand(
                Nom: "ValidName",
                Tel: tel,
                Adresse: null,
                Matricule: null,
                Code: null,
                CodeCat: null,
                EtbSec: null,
                Mail: null);
            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.Tel)
                  .WithErrorMessage("tel_is_required");
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("abcdefghijk")]
        [InlineData("+123456789")] // 9 digits only, too short
        [InlineData("+12345678901234567")] // 17 digits, too long
        public void Should_Have_Error_When_Tel_Format_Invalid(string tel)
        {
            var model = new CreateCustomerCommand(
                Nom: "ValidName",
                Tel: tel,
                Adresse: null,
                Matricule: null,
                Code: null,
                CodeCat: null,
                EtbSec: null,
                Mail: null);

            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.Tel)
                  .WithErrorMessage("tel_must_be_heigher_than_10_and_less_than_15");
        }

        [Fact]
        public void Should_Have_Error_When_Mail_Invalid()
        {
            var model = new CreateCustomerCommand(
                Nom: "ValidName",
                Tel: "+12345678901",
                Adresse: null,
                Matricule: null,
                Code: null,
                CodeCat: null,
                EtbSec: null,
                Mail: "not-an-email");
            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.Mail)
                  .WithErrorMessage("mail_must_be_a_valid_email_address");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new CreateCustomerCommand(
                Nom: "ValidName",
                Tel: "+12345678901",
                Adresse: "123 Main St",
                Matricule: "Mat123",
                Code: "Code123",
                CodeCat: "Cat123",
                EtbSec: "EtabSec",
                Mail: "test@example.com");
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
