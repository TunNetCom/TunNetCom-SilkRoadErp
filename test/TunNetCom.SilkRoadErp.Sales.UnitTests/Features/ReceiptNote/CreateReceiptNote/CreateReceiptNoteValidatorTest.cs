using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Validators.ReceiptNote
{
    public class CreateReceiptNoteValidatorTest
    {
        private readonly CreateReceiptNoteValidator _validator;

        public CreateReceiptNoteValidatorTest()
        {
            _validator = new CreateReceiptNoteValidator();
        }

        [Fact]
        public void Should_HaveError_When_Num_IsEmpty()
        {
            var model = new CreateReceiptNoteCommand(0, 123, DateTime.Today, 1, DateTime.Today, null);
            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.Num)
                  .WithErrorMessage("number_is_required");
        }

        [Fact]
        public void Should_HaveError_When_NumBonFournisseur_IsEmpty()
        {
            var model = new CreateReceiptNoteCommand(1, 0, DateTime.Today, 1, DateTime.Today, null);
            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.NumBonFournisseur)
                  .WithErrorMessage("provider_receipt_number_is_required");
        }

        [Fact]
        public void Should_HaveError_When_DateLivraison_IsDefault()
        {
            var model = new CreateReceiptNoteCommand(1, 123, default, 1, DateTime.Today, null);
            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.DateLivraison)
                  .WithErrorMessage("delivery_date_is_required");
        }

        [Fact]
        public void Should_HaveError_When_IdFournisseur_IsNegative()
        {
            var model = new CreateReceiptNoteCommand(1, 123, DateTime.Today, -1, DateTime.Today, null);
            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.IdFournisseur)
                  .WithErrorMessage("providerid_must_be_greater_than_or_equal_to_0");
        }

        [Fact]
        public void Should_HaveError_When_Date_IsDefault()
        {
            var model = new CreateReceiptNoteCommand(1, 123, DateTime.Today, 1, default, null);
            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.Date)
                  .WithErrorMessage("date_is_required");
        }

        [Fact]
        public void Should_HaveError_When_NumFactureFournisseur_IsNegative()
        {
            var model = new CreateReceiptNoteCommand(1, 123, DateTime.Today, 1, DateTime.Today, -5);
            var result = _validator.TestValidate(model);
            _ = result.ShouldHaveValidationErrorFor(x => x.NumFactureFournisseur)
                  .WithErrorMessage("invoice_number_must_be_greater_than_or_equal_to_0");
        }

        [Fact]
        public void Should_NotHaveAnyValidationErrors_When_CommandIsValid()
        {
            var model = new CreateReceiptNoteCommand(1, 123456, DateTime.Today, 1, DateTime.Today, 999);
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
