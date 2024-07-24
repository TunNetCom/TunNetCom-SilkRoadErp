namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.CreateProvider;

public class CreateProviderValidator : AbstractValidator<CreateProviderCommand>
{
    public CreateProviderValidator()
    {
        RuleFor(x => x.Nom)
             .NotEmpty().WithMessage("Name_is_required")
             .MaximumLength(50).WithMessage("Name_must_be_less_than_50_characters");

        RuleFor(x => x.Tel)
            .NotEmpty().WithMessage("Mobile_number_is_required")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Mobile_number_must_be_heigher_than_10_and_less_than_15_numbers");

        RuleFor(x => x.Fax)
           .MaximumLength(50).WithMessage("Fax_must_be_less_than_50_characters");

        RuleFor(x => x.Matricule)
            .MaximumLength(50).WithMessage("Matricule_must_be_less_than_50_characters");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Code_must_be_less_than_50_characters");

        RuleFor(x => x.CodeCat)
            .MaximumLength(50).WithMessage("CodeCat_must_be_less_than_50_characters");

        RuleFor(x => x.EtbSec)
           .MaximumLength(50).WithMessage("EtbSec_must_be_less_than_50_characters");

        RuleFor(x => x.Mail)
                   .MaximumLength(50).WithMessage("Mail_must_be_less_than_50_characters")
                   .EmailAddress().WithMessage("Mail_must_be_a_valid_email_address");

        RuleFor(x => x.MailDeux)
           .MaximumLength(50).WithMessage("Mail_must_be_less_than_50_characters")
           .EmailAddress().WithMessage("Mail_must_be_a_valid_email_address");

        RuleFor(x => x.Constructeur)
           .NotEmpty().WithMessage("constructor_must_be_a_valid_boolean_value_true_or_false");

        RuleFor(x => x.Adresse)
            .MaximumLength(50).WithMessage("Adress_must_be_less_than_50_characters");
    }
}
