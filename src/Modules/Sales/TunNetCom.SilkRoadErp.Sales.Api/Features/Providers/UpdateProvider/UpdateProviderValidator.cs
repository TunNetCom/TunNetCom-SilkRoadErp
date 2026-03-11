namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.UpdateProvider;

public class UpdateProviderValidator : AbstractValidator<UpdateProviderCommand>
{
    public UpdateProviderValidator()
    {
        _ = RuleFor(x => x.Nom)
             .NotEmpty().WithMessage("name_is_required")
             .MaximumLength(50).WithMessage("name_must_be_less_than_50_characters");

        _ = RuleFor(x => x.Tel)
            .NotEmpty().WithMessage("mobile_number_is_required")
            .Matches(@"^\+?\d{8,15}$").WithMessage("mobile_number_must_be_between_8_and_15_digits");

        _ = RuleFor(x => x.Fax)
           .MaximumLength(50).WithMessage("fax_must_be_less_than_50_characters");

        _ = RuleFor(x => x.Matricule)
            .MaximumLength(50).WithMessage("matricule_must_be_less_than_50_characters");

        _ = RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("code_must_be_less_than_50_characters");

        _ = RuleFor(x => x.CodeCat)
            .MaximumLength(50).WithMessage("codeCat_must_be_less_than_50_characters");

        _ = RuleFor(x => x.EtbSec)
           .MaximumLength(50).WithMessage("etbSec_must_be_less_than_50_characters");

        _ = RuleFor(x => x.Mail)
            .EmailAddress().WithMessage("mail_must_be_a_valid_email_address")
            .When(x => !string.IsNullOrEmpty(x.Mail));

        _ = RuleFor(x => x.MailDeux)
            .EmailAddress().WithMessage("mail_must_be_a_valid_email_address")
            .When(x => !string.IsNullOrEmpty(x.MailDeux));

        _ = RuleFor(x => x.Adresse)
            .MaximumLength(50).WithMessage("adress_must_be_less_than_50_characters");
    }
}
