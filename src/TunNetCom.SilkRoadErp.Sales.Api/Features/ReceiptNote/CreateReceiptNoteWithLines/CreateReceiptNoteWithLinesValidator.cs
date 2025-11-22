using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteWithLines;

public class CreateReceiptNoteWithLinesValidator : AbstractValidator<CreateReceiptNoteWithLinesRequest>
{
    public CreateReceiptNoteWithLinesValidator()
    {
        _ = RuleFor(x => x.IdFournisseur)
            .NotEmpty()
            .WithMessage("Provider ID is required");

        _ = RuleFor(x => x.ReceiptNoteLines)
            .NotEmpty()
            .WithMessage("Receipt note lines are required")
            .Must(lines => lines != null && lines.Any())
            .WithMessage("At_least_one_receipt_note_line_is_required");

        _ = RuleForEach(x => x.ReceiptNoteLines)
            .ChildRules(line =>
            {
                _ = line.RuleFor(x => x.ProductRef)
                    .NotEmpty()
                    .WithMessage("Article_ID_is_required");

                _ = line.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity_must_be_greater_than_0");

                _ = line.RuleFor(x => x.UnitPrice)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Unit_price_must_be_greater_than_or_equal_to_0");
            });
    }
}
