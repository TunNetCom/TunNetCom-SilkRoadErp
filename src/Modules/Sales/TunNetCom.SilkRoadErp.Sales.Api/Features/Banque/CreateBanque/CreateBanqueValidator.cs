using TunNetCom.SilkRoadErp.Sales.Contracts.Banque;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Banque.CreateBanque;

public class CreateBanqueValidator : AbstractValidator<CreateBanqueCommand>
{
    public CreateBanqueValidator()
    {
        _ = RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("nom_is_required")
            .MaximumLength(200).WithMessage("nom_max_length_200");
    }
}

