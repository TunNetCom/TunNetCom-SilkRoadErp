using FluentValidation;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteLines
{
    public class CreateReceiptNoteValidator : AbstractValidator<CreateReceiptNoteLineRequest>
    {
        public CreateReceiptNoteValidator()
        {
            _ = RuleFor(x => x.ProductRef)
                .NotEmpty()
                .WithMessage("Product ID is required");

            _ = RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero");

            _ = RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Unit price must be greater than or equal to zero");

            _ = RuleFor(x => x.ProductDescription)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters");

            _ = RuleFor(x => x.RecipetNoteNumber)
                .NotEmpty()
                .WithMessage("Receipt Note ID is required");
        }
    }
}
