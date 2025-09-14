namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.UpdateProduct;
public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        _ = RuleFor(x => x.Refe)
            .NotEmpty().WithMessage("reference_required")
            .MaximumLength(50).WithMessage("reference_must_be_less_than_50_characters");
        _ = RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("nom_required")
            .MaximumLength(50).WithMessage("nom_must_be_less_than_50_characters");
        _ = RuleFor(x => x.Qte)
            .GreaterThanOrEqualTo(0).WithMessage("quantite_must_be_non_negative");

        _ = RuleFor(x => x.QteLimite)
            .GreaterThanOrEqualTo(0).WithMessage("quantite_limit_must_be_non_negative");

        _ = RuleFor(x => x.Remise)
            .InclusiveBetween(0f, 100f).WithMessage("remise_must_be_between_0_and_100");

        _ = RuleFor(x => x.RemiseAchat)
            .InclusiveBetween(0f, 100f).WithMessage("remise_achat_must_be_between_0_and_100");

        _ = RuleFor(x => x.Tva)
            .InclusiveBetween(0f, 100f).WithMessage("tva_must_be_between_0_and_100");

        _ = RuleFor(x => x.Prix)
            .GreaterThan(0).WithMessage("prix_must_be_greater_than_0");

        _ = RuleFor(x => x.PrixAchat)
            .GreaterThan(0).WithMessage("prix_achat_must_be_greater_than_0");

        _ = RuleFor(x => x.Visibilite)
            .NotNull().WithMessage("visibilite_must_be_specified");
    }
}