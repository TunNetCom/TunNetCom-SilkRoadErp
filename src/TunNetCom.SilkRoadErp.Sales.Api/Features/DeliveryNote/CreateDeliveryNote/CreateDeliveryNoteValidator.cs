namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public class CreateDeliveryNoteValidator : AbstractValidator<CreateDeliveryNoteCommand>
{
    public CreateDeliveryNoteValidator() 
    {
        RuleFor(x => x.Date)
               .NotEmpty().WithMessage("Date is required");

        RuleFor(x => x.TotHTva)
               .GreaterThanOrEqualTo(0).WithMessage("totHTva_must_be_greater_than_or_equal_to_0");

        RuleFor(x => x.TotTva)
               .GreaterThanOrEqualTo(0).WithMessage("TotTva_must_be_greater_than_or_equal_to_0");

        RuleFor(x => x.NetPayer)
                .GreaterThanOrEqualTo(0).WithMessage("NetPayer_must_be_greater_than_or_equal_to_0");

        RuleFor(x => x.TempBl)
               .NotEmpty().WithMessage("TempBl is required");

        RuleFor(x => x.NumFacture)
               .GreaterThanOrEqualTo(0).WithMessage("NumFacture_must_be_greater_than_or_equal_to_0")
               .When(x => x.NumFacture.HasValue);

        RuleFor(x => x.ClientId)
                .GreaterThanOrEqualTo(0).WithMessage("ClientId_must_be_greater_than_or_equal_to_0")
                .When(x => x.ClientId.HasValue);
    }
}
