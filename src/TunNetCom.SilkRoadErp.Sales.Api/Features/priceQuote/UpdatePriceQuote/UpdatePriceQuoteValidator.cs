namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.UpdatePriceQuote;

public class UpdatePriceQuoteValidator : AbstractValidator<UpdatePriceQuoteCommand>
{
    public UpdatePriceQuoteValidator()
    {
        RuleFor(x => x.Num)
                   .NotEmpty().WithMessage("num_is_required");

        RuleFor(x => x.IdClient)
            .NotEmpty().WithMessage("client_id_is_required");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("date_is_required")
            .Must(BeAValidDate).WithMessage("invalid_date_format");

        RuleFor(x => x.TotHTva)
            .GreaterThanOrEqualTo(0).WithMessage("total_ht_tva_must_be_greater_or_equal_to_zero");

        RuleFor(x => x.TotTva)
            .GreaterThanOrEqualTo(0).WithMessage("total_tva_must_be_greater_or_equal_to_zero");

        RuleFor(x => x.TotTtc)
            .GreaterThanOrEqualTo(0).WithMessage("total_ttc_must_be_greater_or_equal_to_zero");
    }

    private bool BeAValidDate(DateTime date)
    {
        return date != default(DateTime);
    }
}
