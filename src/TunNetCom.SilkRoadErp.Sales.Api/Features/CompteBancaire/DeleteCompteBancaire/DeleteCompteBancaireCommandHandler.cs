namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.DeleteCompteBancaire;

public class DeleteCompteBancaireCommandHandler(
    SalesContext _context,
    ILogger<DeleteCompteBancaireCommandHandler> _logger)
    : IRequestHandler<DeleteCompteBancaireCommand, Result>
{
    public async Task<Result> Handle(DeleteCompteBancaireCommand command, CancellationToken cancellationToken)
    {
        var compte = await _context.CompteBancaire.FindAsync([command.Id], cancellationToken);
        if (compte == null)
        {
            return Result.Fail("compte_bancaire_not_found");
        }

        var hasImports = await _context.BankTransactionImport.AnyAsync(i => i.CompteBancaireId == command.Id, cancellationToken);
        if (hasImports)
        {
            return Result.Fail("compte_bancaire_has_imports_cannot_delete");
        }

        _context.CompteBancaire.Remove(compte);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("CompteBancaire {Id} deleted", command.Id);
        return Result.Ok();
    }
}
