using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.UpdatePaiementFournisseur;

public class UpdatePaiementFournisseurCommandHandler(
    SalesContext _context,
    ILogger<UpdatePaiementFournisseurCommandHandler> _logger)
    : IRequestHandler<UpdatePaiementFournisseurCommand, Result>
{
    public async Task<Result> Handle(UpdatePaiementFournisseurCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdatePaiementFournisseurCommand called with Id {Id}", command.Id);

        var paiement = await _context.PaiementFournisseur
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (paiement == null)
        {
            return Result.Fail("paiement_fournisseur_not_found");
        }

        // Validate numero is unique (if changed)
        if (paiement.Numero != command.Numero)
        {
            var numeroExists = await _context.PaiementFournisseur
                .AnyAsync(p => p.Numero == command.Numero && p.Id != command.Id, cancellationToken);
            if (numeroExists)
            {
                return Result.Fail("numero_already_exists");
            }
        }

        // Validate MethodePaiement enum
        if (!Enum.TryParse<MethodePaiement>(command.MethodePaiement, out var methodePaiement))
        {
            return Result.Fail("invalid_methode_paiement");
        }

        // Get active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            return Result.Fail("no_active_accounting_year");
        }

        // Validate document links if provided
        if (command.FactureFournisseurId.HasValue)
        {
            var factureExists = await _context.FactureFournisseur.AnyAsync(f => f.Id == command.FactureFournisseurId.Value, cancellationToken);
            if (!factureExists)
            {
                return Result.Fail("facture_fournisseur_not_found");
            }
        }

        if (command.BonDeReceptionId.HasValue)
        {
            var bonDeReceptionExists = await _context.BonDeReception.AnyAsync(b => b.Id == command.BonDeReceptionId.Value, cancellationToken);
            if (!bonDeReceptionExists)
            {
                return Result.Fail("bon_de_reception_not_found");
            }
        }

        // Validate banque if provided
        if (command.BanqueId.HasValue)
        {
            var banqueExists = await _context.Banque.AnyAsync(b => b.Id == command.BanqueId.Value, cancellationToken);
            if (!banqueExists)
            {
                return Result.Fail("banque_not_found");
            }
        }

        paiement.UpdatePaiementFournisseur(
            command.Numero,
            command.FournisseurId,
            activeAccountingYear.Id,
            command.Montant,
            command.DatePaiement,
            methodePaiement,
            command.FactureFournisseurId,
            command.BonDeReceptionId,
            command.NumeroChequeTraite,
            command.BanqueId,
            command.DateEcheance,
            command.Commentaire,
            command.RibCodeEtab,
            command.RibCodeAgence,
            command.RibNumeroCompte,
            command.RibCle);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PaiementFournisseur with Id {Id} updated successfully", command.Id);
        return Result.Ok();
    }
}

