using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.CreateCompteBancaire;

public class CreateCompteBancaireValidator : AbstractValidator<CreateCompteBancaireCommand>
{
    public CreateCompteBancaireValidator()
    {
        _ = RuleFor(x => x.BanqueId).GreaterThan(0).WithMessage("banque_is_required");
        _ = RuleFor(x => x.CodeEtablissement).NotEmpty().WithMessage("code_etablissement_is_required").MaximumLength(10).WithMessage("code_etablissement_max_10");
        _ = RuleFor(x => x.CodeAgence).NotEmpty().WithMessage("code_agence_is_required").MaximumLength(10).WithMessage("code_agence_max_10");
        _ = RuleFor(x => x.NumeroCompte).NotEmpty().WithMessage("numero_compte_is_required").MaximumLength(20).WithMessage("numero_compte_max_20");
        _ = RuleFor(x => x.CleRib).NotEmpty().WithMessage("cle_rib_is_required").MaximumLength(5).WithMessage("cle_rib_max_5");
        _ = RuleFor(x => x.Libelle).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Libelle));
    }
}
