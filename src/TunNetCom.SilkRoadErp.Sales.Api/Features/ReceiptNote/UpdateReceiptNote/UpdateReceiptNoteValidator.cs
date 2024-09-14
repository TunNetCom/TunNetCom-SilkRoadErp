namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.UpdateReceiptNote;

public class UpdateReceiptNoteValidator : AbstractValidator<UpdateReceiptNoteCommand>
{
    public UpdateReceiptNoteValidator()
    {
        RuleFor(x => x.Num)
             .NotEmpty().WithMessage("number_is_required");
        RuleFor(x => x.NumBonFournisseur)
            .NotEmpty().WithMessage("provider_receipt_number_is_required");
        RuleFor(x => x.DateLivraison)
                .NotEmpty().WithMessage("delivery_date_is_required");
        RuleFor(x => x.IdFournisseur)
                .NotEmpty().WithMessage("providerid_is_required")
                .GreaterThanOrEqualTo(0).WithMessage("providerid_must_be_greater_than_or_equal_to_0");
        RuleFor(x => x.Date)
                .NotEmpty().WithMessage("date_is_required");
        RuleFor(x => x.NumFactureFournisseur)
             .GreaterThanOrEqualTo(0).WithMessage("invoice_number_must_be_greater_than_or_equal_to_0")
               .When(x => x.NumFactureFournisseur.HasValue);


    }
}