namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.UpdateProvider;

public class UpdateProviderValidator : AbstractValidator<UpdateProviderCommand>
{
    public UpdateProviderValidator()
    {
        RuleFor(x => x.Nom)
             .NotEmpty().WithMessage("name_is_required")
             .MaximumLength(50).WithMessage("name_must_be_less_than_50_characters");

        RuleFor(x => x.Tel)
            .NotEmpty().WithMessage("mobile_number_is_required")
            .Matches(@"^\+?\d{10,15}$").WithMessage("mobile_number_must_be_heigher_than_10_and_less_than_15_numbers");

        RuleFor(x => x.Fax)
           .MaximumLength(50).WithMessage("fax_must_be_less_than_50_characters");

        RuleFor(x => x.Matricule)
            .MaximumLength(50).WithMessage("matricule_must_be_less_than_50_characters");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("code_must_be_less_than_50_characters");

        RuleFor(x => x.CodeCat)
            .MaximumLength(50).WithMessage("codeCat_must_be_less_than_50_characters");

        RuleFor(x => x.EtbSec)
           .MaximumLength(50).WithMessage("etbSec_must_be_less_than_50_characters");

        RuleFor(x => x.Mail)
                   .MaximumLength(50).WithMessage("mail_must_be_less_than_50_characters")
                   .EmailAddress().WithMessage("mail_must_be_a_valid_email_address");

        RuleFor(x => x.MailDeux)
           .MaximumLength(50).WithMessage("mail_must_be_less_than_50_characters")
           .EmailAddress().WithMessage("mail_must_be_a_valid_email_address");

        RuleFor(x => x.Constructeur)
           .NotEmpty().WithMessage("constructor_must_be_a_valid_boolean_value_true_or_false");

        RuleFor(x => x.Adresse)
            .MaximumLength(50).WithMessage("adress_must_be_less_than_50_characters");
    }
}
