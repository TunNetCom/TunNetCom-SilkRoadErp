namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.UpdateProvider;

public class UpdateProviderValidator : AbstractValidator<UpdateProviderCommand>
{
    public UpdateProviderValidator()
    {
        RuleFor(x => x.Nom)
             .NotEmpty().WithMessage("Name is required")
             .MaximumLength(50).WithMessage("Name must be less than 50 characters");

        RuleFor(x => x.Tel)
            .NotEmpty().WithMessage("Mobile number is required")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Mobile number must be heigher than 10 and less than 15");

        RuleFor(x => x.Fax)
            .NotEmpty().WithMessage("Fax is required")
           .MaximumLength(50).WithMessage("Fax must be less than 50 characters");

        RuleFor(x => x.Matricule)
            .MaximumLength(50).WithMessage("Matricule must be less than 50 characters");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Code must be less than 50 characters");

        RuleFor(x => x.CodeCat)
            .MaximumLength(50).WithMessage("CodeCat must be less than 50 characters");

        RuleFor(x => x.EtbSec)
           .MaximumLength(50).WithMessage("EtbSec must be less than 50 characters");

        RuleFor(x => x.Mail)
                   .MaximumLength(50).WithMessage("Mail must be less than 50 characters")
                   .EmailAddress().WithMessage("Mail must be a valid email address");

        RuleFor(x => x.MailDeux)
           .MaximumLength(50).WithMessage("Mail must be less than 50 characters")
           .EmailAddress().WithMessage("Mail must be a valid email address");

        RuleFor(x => x.Constructeur)
    .NotEmpty()
    .WithMessage("constructor must be a valid boolean value (true or false).");


        RuleFor(x => x.Adresse)
            .MaximumLength(50).WithMessage("Adress must be less than 50 characters");

    }
}
