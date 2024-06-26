namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.CreateClient;

public class CreateClientValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Nom is required")
            .MaximumLength(50).WithMessage("Nom must be less than 50 characters");

        RuleFor(x => x.Tel)
            .NotEmpty().WithMessage("Tel is required")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Tel must be a valid phone number");

        RuleFor(x => x.Adresse)
            .MaximumLength(50).WithMessage("Adresse must be less than 50 characters");

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
    }
}
