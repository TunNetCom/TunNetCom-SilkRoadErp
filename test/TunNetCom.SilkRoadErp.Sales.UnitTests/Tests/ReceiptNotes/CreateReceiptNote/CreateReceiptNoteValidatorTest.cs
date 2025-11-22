//using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
//namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
//{
//    public class CreateReceiptNoteValidatorTest
//    {
//        private readonly CreateReceiptNoteValidator _validator;
//        public CreateReceiptNoteValidatorTest()
//        {
//            _validator = new CreateReceiptNoteValidator();
//        }
//        [Fact]
//        public void Should_Have_Error_When_Num_Is_Zero()
//        {
//            var command = new CreateReceiptNoteCommand(
//                Num: 0, // zero => NotEmpty will fail for int (default)
//                NumBonFournisseur: 123456,
//                DateLivraison: DateTime.Now,
//                IdFournisseur: 1,
//                Date: DateTime.Now,
//                NumFactureFournisseur: null
//            );
//            var result = _validator.TestValidate(command);
//            _ = result.ShouldHaveValidationErrorFor(x => x.Num)
//                .WithErrorMessage("number_is_required");
//        }

//        [Fact]
//        public void Should_Not_Have_Error_When_Valid_Command()
//        {
//            var command = new CreateReceiptNoteCommand(
//                Num: 1,
//                NumBonFournisseur: 123456,
//                DateLivraison: DateTime.Now,
//                IdFournisseur: 10,
//                Date: DateTime.Now,
//                NumFactureFournisseur: 100
//            );
//            var result = _validator.TestValidate(command);
//            result.ShouldNotHaveAnyValidationErrors();
//        }

//        [Fact]
//        public void Should_Have_Error_When_IdFournisseur_Is_Negative()
//        {
//            var command = new CreateReceiptNoteCommand(
//                Num: 1,
//                NumBonFournisseur: 654321,
//                DateLivraison: DateTime.Now,
//                IdFournisseur: -1, // erreur attendue
//                Date: DateTime.Now,
//                NumFactureFournisseur: null
//            );
//            var result = _validator.TestValidate(command);
//            _ = result.ShouldHaveValidationErrorFor(x => x.IdFournisseur)
//                .WithErrorMessage("providerid_must_be_greater_than_or_equal_to_0");
//        }

//        [Fact]
//        public void Should_Have_Error_When_NumFactureFournisseur_Is_Negative()
//        {
//            var command = new CreateReceiptNoteCommand(
//                Num: 1,
//                NumBonFournisseur: 111111,
//                DateLivraison: DateTime.Now,
//                IdFournisseur: 5,
//                Date: DateTime.Now,
//                NumFactureFournisseur: -10 // erreur attendue
//            );
//            var result = _validator.TestValidate(command);
//            _ = result.ShouldHaveValidationErrorFor(x => x.NumFactureFournisseur)
//                .WithErrorMessage("invoice_number_must_be_greater_than_or_equal_to_0");
//        }
//    }
//}
