using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.UpdateAvoir;

public class UpdateAvoirValidator : AbstractValidator<UpdateAvoirCommand>
{
    public UpdateAvoirValidator()
    {
        _ = RuleFor(x => x.Num)
            .GreaterThan(0).WithMessage("num_must_be_greater_than_zero");

        _ = RuleFor(x => x.Date)
            .NotEmpty().WithMessage("date_is_required");

        _ = RuleFor(x => x.Lines)
            .NotEmpty().WithMessage("lines_are_required")
            .Must(lines => lines.Count > 0).WithMessage("at_least_one_line_is_required");

        _ = RuleForEach(x => x.Lines)
            .SetValidator(new AvoirLineValidator());
    }
}

public class AvoirLineValidator : AbstractValidator<AvoirLineRequest>
{
    public AvoirLineValidator()
    {
        _ = RuleFor(x => x.RefProduit)
            .NotEmpty().WithMessage("ref_produit_is_required");

        _ = RuleFor(x => x.QteLi)
            .GreaterThan(0).WithMessage("qte_li_must_be_greater_than_zero");

        _ = RuleFor(x => x.PrixHt)
            .GreaterThanOrEqualTo(0).WithMessage("prix_ht_must_be_greater_than_or_equal_to_zero");

        _ = RuleFor(x => x.Remise)
            .GreaterThanOrEqualTo(0).WithMessage("remise_must_be_greater_than_or_equal_to_zero")
            .LessThanOrEqualTo(100).WithMessage("remise_must_be_less_than_or_equal_to_100");

        _ = RuleFor(x => x.Tva)
            .GreaterThanOrEqualTo(0).WithMessage("tva_must_be_greater_than_or_equal_to_zero");
    }
}

