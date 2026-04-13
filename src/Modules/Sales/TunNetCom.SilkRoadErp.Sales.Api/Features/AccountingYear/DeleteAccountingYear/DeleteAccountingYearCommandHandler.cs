using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.DeleteAccountingYear;

public class DeleteAccountingYearCommandHandler(
    SalesContext _context,
    ILogger<DeleteAccountingYearCommandHandler> _logger)
    : IRequestHandler<DeleteAccountingYearCommand, Result>
{
    public async Task<Result> Handle(DeleteAccountingYearCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting accounting year with Id {AccountingYearId}", command.Id);

        var accountingYear = await _context.AccountingYear
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(ay => ay.Id == command.Id, cancellationToken);

        if (accountingYear == null)
        {
            return Result.Fail("accounting_year_not_found");
        }

        // Vérifier si l'exercice est utilisé (a des relations)
        var isUsed = await _context.Facture.AnyAsync(f => f.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.FactureFournisseur.AnyAsync(f => f.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.BonDeLivraison.AnyAsync(b => b.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.BonDeReception.AnyAsync(b => b.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.Avoirs.AnyAsync(a => a.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.AvoirFournisseur.AnyAsync(a => a.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.FactureAvoirFournisseur.AnyAsync(f => f.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.FactureAvoirClient.AnyAsync(f => f.AccountingYearId == command.Id, cancellationToken) ||
                     await _context.Inventaire.AnyAsync(i => i.AccountingYearId == command.Id, cancellationToken);

        if (isUsed)
        {
            _logger.LogWarning("Cannot delete accounting year {Id} because it is used", command.Id);
            return Result.Fail("accounting_year_used_cannot_delete");
        }

        _context.AccountingYear.Remove(accountingYear);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Accounting year {AccountingYearId} deleted successfully", command.Id);

        return Result.Ok();
    }
}
