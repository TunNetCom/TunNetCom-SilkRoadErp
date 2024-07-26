namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Nom)
             .NotEmpty().WithMessage("nom_is_required")
             .MaximumLength(50).WithMessage("nom_must_be_less_than_50_characters");

        RuleFor(x => x.Tel)
            .NotEmpty().WithMessage("tel_is_required")
            .Matches(@"^\+?\d{10,15}$").WithMessage("tel_must_be_heigher_than_10_and_less_than_15");

        RuleFor(x => x.Adresse)
            .MaximumLength(50).WithMessage("adresse_must_be_less_than_50_characters");

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
    }
}
