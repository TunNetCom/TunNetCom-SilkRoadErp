namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Nom)
             .NotEmpty().WithMessage("Nom is required")
             .MaximumLength(50).WithMessage("Nom must be less than 50 characters");

        RuleFor(x => x.Tel)
            .NotEmpty().WithMessage("Tel is required")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Tel must be heigher than 10 and less than 15");

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
