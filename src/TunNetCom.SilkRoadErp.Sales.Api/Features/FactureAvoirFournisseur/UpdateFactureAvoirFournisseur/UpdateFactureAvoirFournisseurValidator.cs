namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.UpdateFactureAvoirFournisseur;

public class UpdateFactureAvoirFournisseurValidator : AbstractValidator<UpdateFactureAvoirFournisseurCommand>
{
    public UpdateFactureAvoirFournisseurValidator()
    {
        _ = RuleFor(x => x.Num)
            .GreaterThan(0).WithMessage("num_must_be_greater_than_zero");

        _ = RuleFor(x => x.Date)
            .NotEmpty().WithMessage("date_is_required");

        _ = RuleFor(x => x.IdFournisseur)
            .GreaterThan(0).WithMessage("id_fournisseur_must_be_greater_than_zero");

        _ = RuleFor(x => x.AvoirFournisseurIds)
            .NotEmpty().WithMessage("avoir_fournisseur_ids_are_required")
            .Must(ids => ids.Count > 0).WithMessage("at_least_one_avoir_fournisseur_is_required");
    }
}

