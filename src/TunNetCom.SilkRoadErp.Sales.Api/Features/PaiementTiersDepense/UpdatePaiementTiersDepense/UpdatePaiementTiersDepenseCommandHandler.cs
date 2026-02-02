using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.UpdatePaiementTiersDepense;

public class UpdatePaiementTiersDepenseCommandHandler(SalesContext _context, ILogger<UpdatePaiementTiersDepenseCommandHandler> _logger)
    : IRequestHandler<UpdatePaiementTiersDepenseCommand, Result>
{
    public async Task<Result> Handle(UpdatePaiementTiersDepenseCommand command, CancellationToken cancellationToken)
    {
        var entity = await _context.PaiementTiersDepense
            .Include(p => p.FactureDepenses)
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (entity == null)
        {
            _logger.LogWarning("PaiementTiersDepense not found: Id {Id}", command.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        var tiersExists = await _context.TiersDepenseFonctionnement.AnyAsync(t => t.Id == command.TiersDepenseFonctionnementId, cancellationToken);
        if (!tiersExists)
        {
            _logger.LogWarning("TiersDepenseFonctionnement not found: Id {Id}", command.TiersDepenseFonctionnementId);
            return Result.Fail(EntityNotFound.Error());
        }

        var accountingYear = command.AccountingYearId.HasValue
            ? await _context.AccountingYear.FirstOrDefaultAsync(ay => ay.Id == command.AccountingYearId.Value, cancellationToken)
            : await _context.AccountingYear.FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (accountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        if (!Enum.TryParse<MethodePaiement>(command.MethodePaiement, ignoreCase: true, out var methodePaiement))
        {
            _logger.LogWarning("Invalid MethodePaiement: {Value}", command.MethodePaiement);
            return Result.Fail("invalid_methode_paiement");
        }

        entity.Update(
            command.NumeroTransactionBancaire,
            command.TiersDepenseFonctionnementId,
            accountingYear.Id,
            command.Montant,
            command.DatePaiement,
            methodePaiement,
            command.FactureDepenseIds,
            command.NumeroChequeTraite,
            command.BanqueId,
            command.DateEcheance,
            command.Commentaire,
            command.RibCodeEtab,
            command.RibCodeAgence,
            command.RibNumeroCompte,
            command.RibCle,
            command.Mois);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("PaiementTiersDepense updated: Id {Id}", command.Id);
        return Result.Ok();
    }
}
