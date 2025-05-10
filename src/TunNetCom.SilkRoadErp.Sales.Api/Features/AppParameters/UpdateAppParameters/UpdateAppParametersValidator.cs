namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.UpdateAppParameters;

public class UpdateAppParametersValidator : AbstractValidator<UpdateAppParametersCommand>
{
    public UpdateAppParametersValidator()
    {
        RuleFor(x => x.NomSociete)
            .NotEmpty().WithMessage("app_name_is_required")
            .MaximumLength(50).WithMessage("app_name_must_be_less_than_50_characters");

        RuleFor(x => x.Tel)
            .NotEmpty().WithMessage("tel_is_required");

        RuleFor(x => x.Adresse)
            .NotEmpty().WithMessage("adresse_is_required")
            .MaximumLength(50).WithMessage("adresse_must_be_less_than_50_characters");

        RuleFor(x => x.Email)
            .MaximumLength(50).WithMessage("email_must_be_less_than_50_characters")
            .EmailAddress().WithMessage("email_must_be_a_valid_email_address");

        RuleFor(x => x.MatriculeFiscale)
            .MaximumLength(50).WithMessage("matriculeFiscale_must_be_less_than_50_characters");

        RuleFor(x => x.CodeTva)
            .MaximumLength(50).WithMessage("codeTva_must_be_less_than_50_characters");

        RuleFor(x => x.CodeCategorie)
            .MaximumLength(50).WithMessage("codeCategorie_must_be_less_than_50_characters");
        
        RuleFor(x => x.EtbSecondaire)
            .MaximumLength(50).WithMessage("etbSecondaire_must_be_less_than_50_characters");

        RuleFor(x => x.AdresseRetenu)
            .MaximumLength(50).WithMessage("adresseRetenu_must_be_less_than_50_characters");

    }
}
