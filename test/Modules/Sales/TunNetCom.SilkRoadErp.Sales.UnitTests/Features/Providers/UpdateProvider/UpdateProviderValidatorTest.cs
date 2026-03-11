//namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Providers
//{
//    public class UpdateProviderValidatorTest
//    {
//        private readonly UpdateProviderValidator _validator = new();

//        private UpdateProviderCommand GetValidCommand() => new(
//            Id: 1,
//            Nom: "Valid Name",
//            Tel: "+21612345678",
//            Fax: "123456",
//            Matricule: "MAT123",
//            Code: "C123",
//            CodeCat: "CCAT",
//            EtbSec: "Sec",
//            Mail: "provider@mail.com",
//            MailDeux: "second@mail.com",
//            Constructeur: true,
//            Adresse: "Tunis"
//        );

//        [Fact]
//        public void Should_Have_Error_When_Nom_Is_Empty()
//        {
//            var command = GetValidCommand() with { Nom = "" };

//            var result = _validator.TestValidate(command);
//            _ = result.ShouldHaveValidationErrorFor(x => x.Nom)
//                  .WithErrorMessage("name_is_required");
//        }

//        [Fact]
//        public void Should_Have_Error_When_Nom_Is_Too_Long()
//        {
//            var command = GetValidCommand() with { Nom = new string('A', 51) };

//            var result = _validator.TestValidate(command);
//            _ = result.ShouldHaveValidationErrorFor(x => x.Nom)
//                  .WithErrorMessage("name_must_be_less_than_50_characters");
//        }

//        [Fact]
//        public void Should_Have_Error_When_Tel_Is_Invalid_Format()
//        {
//            var command = GetValidCommand() with { Tel = "123" };

//            var result = _validator.TestValidate(command);
//            _ = result.ShouldHaveValidationErrorFor(x => x.Tel)
//                  .WithErrorMessage("mobile_number_must_be_heigher_than_10_and_less_than_15_numbers");
//        }

//        [Fact]
//        public void Should_Have_Error_When_Mail_Is_Invalid()
//        {
//            var command = GetValidCommand() with { Mail = "invalid-mail" };

//            var result = _validator.TestValidate(command);
//            _ = result.ShouldHaveValidationErrorFor(x => x.Mail)
//                  .WithErrorMessage("mail_must_be_a_valid_email_address");
//        }

//        [Fact]
//        public void Should_Have_Error_When_Adresse_Is_Too_Long()
//        {
//            var command = GetValidCommand() with { Adresse = new string('A', 51) };

//            var result = _validator.TestValidate(command);
//            _ = result.ShouldHaveValidationErrorFor(x => x.Adresse)
//                  .WithErrorMessage("adress_must_be_less_than_50_characters");
//        }

//        [Fact]
//        public void Should_Not_Have_Errors_When_Command_Is_Valid()
//        {
//            var command = GetValidCommand();

//            var result = _validator.TestValidate(command);
//            result.ShouldNotHaveAnyValidationErrors();
//        }
//    }
//}
