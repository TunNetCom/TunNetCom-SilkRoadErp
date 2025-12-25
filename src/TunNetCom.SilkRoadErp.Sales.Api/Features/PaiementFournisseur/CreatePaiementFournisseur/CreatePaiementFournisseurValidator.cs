using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.CreatePaiementFournisseur;

public class CreatePaiementFournisseurValidator : AbstractValidator<CreatePaiementFournisseurCommand>
{
    public CreatePaiementFournisseurValidator()
    {
        _ = RuleFor(x => x.Numero)
            .NotEmpty().WithMessage("numero_is_required")
            .MaximumLength(50).WithMessage("numero_max_length_50");

        _ = RuleFor(x => x.FournisseurId)
            .GreaterThan(0).WithMessage("fournisseur_id_must_be_greater_than_zero");

        _ = RuleFor(x => x.Montant)
            .GreaterThan(0).WithMessage("montant_must_be_greater_than_zero");

        _ = RuleFor(x => x.DatePaiement)
            .NotEmpty().WithMessage("date_paiement_is_required");

        _ = RuleFor(x => x.MethodePaiement)
            .NotEmpty().WithMessage("methode_paiement_is_required")
            .Must(m => Enum.TryParse<MethodePaiement>(m, out _)).WithMessage("invalid_methode_paiement");

        _ = RuleFor(x => x.NumeroChequeTraite)
            .MaximumLength(100).WithMessage("numero_cheque_traite_max_length_100")
            .When(x => !string.IsNullOrEmpty(x.NumeroChequeTraite));

        _ = RuleFor(x => x.Commentaire)
            .MaximumLength(500).WithMessage("commentaire_max_length_500")
            .When(x => !string.IsNullOrEmpty(x.Commentaire));

        _ = RuleFor(x => x)
            .Must(x => !(x.FactureFournisseurIds != null && x.FactureFournisseurIds.Count > 0 && x.BonDeReceptionIds != null && x.BonDeReceptionIds.Count > 0))
            .WithMessage("cannot_link_to_both_facture_fournisseur_and_bon_de_reception");
    }
}

