namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class UpdateReceiptNoteValidatorTests
    {
        private readonly UpdateReceiptNoteValidator _validator;

        public UpdateReceiptNoteValidatorTests()
        {
            _validator = new UpdateReceiptNoteValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Num_Is_Empty()
        {
            var command = new UpdateReceiptNoteCommand(
                Num: 0,
                NumBonFournisseur: 100,
                DateLivraison: DateTime.Today,
                IdFournisseur: 1,
                Date: DateTime.Today,
                NumFactureFournisseur: 1
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Num)
                  .WithErrorMessage("number_is_required");
        }

        [Fact]
        public void Should_Have_Error_When_NumBonFournisseur_Is_Empty()
        {
            var command = new UpdateReceiptNoteCommand(
                Num: 1,
                NumBonFournisseur: 0,
                DateLivraison: DateTime.Today,
                IdFournisseur: 1,
                Date: DateTime.Today,
                NumFactureFournisseur: 1
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NumBonFournisseur)
                  .WithErrorMessage("provider_receipt_number_is_required");
        }

        [Fact]
        public void Should_Have_Error_When_DateLivraison_Is_Default()
        {
            var command = new UpdateReceiptNoteCommand(
                Num: 1,
                NumBonFournisseur: 100,
                DateLivraison: default,
                IdFournisseur: 1,
                Date: DateTime.Today,
                NumFactureFournisseur: 1
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.DateLivraison)
                  .WithErrorMessage("delivery_date_is_required");
        }

        [Fact]
        public void Should_Have_Error_When_IdFournisseur_Is_Negative()
        {
            var command = new UpdateReceiptNoteCommand(
                Num: 1,
                NumBonFournisseur: 100,
                DateLivraison: DateTime.Today,
                IdFournisseur: -1,
                Date: DateTime.Today,
                NumFactureFournisseur: 1
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.IdFournisseur)
                  .WithErrorMessage("providerid_must_be_greater_than_or_equal_to_0");
        }

        [Fact]
        public void Should_Have_Error_When_Date_Is_Default()
        {
            var command = new UpdateReceiptNoteCommand(
                Num: 1,
                NumBonFournisseur: 100,
                DateLivraison: DateTime.Today,
                IdFournisseur: 1,
                Date: default,
                NumFactureFournisseur: 1
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Date)
                  .WithErrorMessage("date_is_required");
        }

        [Fact]
        public void Should_Have_Error_When_NumFactureFournisseur_Is_Negative()
        {
            var command = new UpdateReceiptNoteCommand(
                Num: 1,
                NumBonFournisseur: 100,
                DateLivraison: DateTime.Today,
                IdFournisseur: 1,
                Date: DateTime.Today,
                NumFactureFournisseur: -1
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.NumFactureFournisseur)
                  .WithErrorMessage("invoice_number_must_be_greater_than_or_equal_to_0");
        }

        [Fact]
        public void Should_Not_Have_Errors_For_Valid_Command()
        {
            var command = new UpdateReceiptNoteCommand(
                Num: 1,
                NumBonFournisseur: 100,
                DateLivraison: DateTime.Today,
                IdFournisseur: 1,
                Date: DateTime.Today,
                NumFactureFournisseur: 10
            );
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

