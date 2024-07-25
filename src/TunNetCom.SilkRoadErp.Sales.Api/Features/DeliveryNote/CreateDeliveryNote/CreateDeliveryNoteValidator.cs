namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public class CreateDeliveryNoteValidator : AbstractValidator<CreateDeliveryNoteCommand>
{
    public CreateDeliveryNoteValidator() 
    {
        RuleFor(x => x.Date)
               .NotEmpty().WithMessage("date_is_required");

        RuleFor(x => x.TotHTva)
               .GreaterThanOrEqualTo(0).WithMessage("tothtva_must_be_greater_than_or_equal_to_0");

        RuleFor(x => x.TotTva)
               .GreaterThanOrEqualTo(0).WithMessage("tottva_must_be_greater_than_or_equal_to_0");

        RuleFor(x => x.NetPayer)
                .GreaterThanOrEqualTo(0).WithMessage("netpayer_must_be_greater_than_or_equal_to_0");

        RuleFor(x => x.TempBl)
               .NotEmpty().WithMessage("tempbl_is_required");

        RuleFor(x => x.NumFacture)
               .GreaterThanOrEqualTo(0).WithMessage("numfacture_must_be_greater_than_or_equal_to_0")
               .When(x => x.NumFacture.HasValue);

        RuleFor(x => x.ClientId)
                .GreaterThanOrEqualTo(0).WithMessage("clientid_must_be_greater_than_or_equal_to_0")
                .When(x => x.ClientId.HasValue);
    }
}
