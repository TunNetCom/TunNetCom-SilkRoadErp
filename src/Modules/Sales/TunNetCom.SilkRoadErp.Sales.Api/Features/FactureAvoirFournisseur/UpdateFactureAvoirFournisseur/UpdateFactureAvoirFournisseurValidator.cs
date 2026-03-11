namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.UpdateFactureAvoirFournisseur;

public class UpdateFactureAvoirFournisseurValidator : AbstractValidator<UpdateFactureAvoirFournisseurCommand>
{
    public UpdateFactureAvoirFournisseurValidator()
    {
        _ = RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("id_must_be_greater_than_zero");

        _ = RuleFor(x => x.Date)
            .NotEmpty().WithMessage("date_is_required");

        _ = RuleFor(x => x.IdFournisseur)
            .GreaterThan(0).WithMessage("id_fournisseur_must_be_greater_than_zero");

        // AvoirFournisseurIds is optional - avoirs can be added later
        _ = RuleFor(x => x.AvoirFournisseurIds)
            .NotNull().WithMessage("avoir_fournisseur_ids_cannot_be_null");
    }
}

