
using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Invoices.CreateInvoice
{
    public class CreateInvoiceValidatorTest
    {
        private readonly CreateInvoiceValidator _validator;
        public CreateInvoiceValidatorTest()
        {
            _validator = new CreateInvoiceValidator();
        }
        [Fact]
        public void Should_Have_Error_When_Date_Is_Empty()
        {
            // Arrange
            var command = new CreateInvoiceCommand(default, 1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            _ = result.ShouldHaveValidationErrorFor(x => x.Date)
                  .WithErrorMessage("date_is_required");
        }
        [Fact]
        public void Should_Have_Error_When_ClientId_Is_Zero()
        {
            // Arrange
            var command = new CreateInvoiceCommand(DateTime.Now, 0);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            _ = result.ShouldHaveValidationErrorFor(x => x.ClientId)
                  .WithErrorMessage("client_id_is_required");
        }
        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Valid()
        {
            // Arrange
            var command = new CreateInvoiceCommand(DateTime.Now, 10);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
