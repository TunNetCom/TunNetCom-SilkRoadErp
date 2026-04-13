namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.CreatePriceQuote;

public class CreatePriceQuoteValidator : AbstractValidator<CreatePriceQuoteCommand>
{
    public CreatePriceQuoteValidator()
    {

        _ = RuleFor(x => x.IdClient)
            .NotEmpty().WithMessage("client_id_is_required");

        _ = RuleFor(x => x.Date)
            .NotEmpty().WithMessage("date_is_required")
            .Must(BeAValidDate).WithMessage("invalid_date_format");

        _ = RuleFor(x => x.TotHTva)
            .GreaterThanOrEqualTo(0).WithMessage("total_ht_tva_must_be_greater_or_equal_to_zero");

        _ = RuleFor(x => x.TotTva)
            .GreaterThanOrEqualTo(0).WithMessage("total_tva_must_be_greater_or_equal_to_zero");

        _ = RuleFor(x => x.TotTtc)
            .GreaterThanOrEqualTo(0).WithMessage("total_ttc_must_be_greater_or_equal_to_zero");
    }

    private bool BeAValidDate(DateTime date)
    {
        return date != default;
    }
}
