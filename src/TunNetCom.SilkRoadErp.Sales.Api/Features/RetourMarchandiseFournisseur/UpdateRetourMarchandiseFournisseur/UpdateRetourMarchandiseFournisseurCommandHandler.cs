using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.UpdateRetourMarchandiseFournisseur;

internal class UpdateRetourMarchandiseFournisseurCommandHandler(
    SalesContext _context,
    ILogger<UpdateRetourMarchandiseFournisseurCommandHandler> _logger)
    : IRequestHandler<UpdateRetourMarchandiseFournisseurCommand, Result>
{
    public async Task<Result> Handle(
        UpdateRetourMarchandiseFournisseurCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(RetourMarchandiseFournisseur), command.Num);

        var retour = await _context.RetourMarchandiseFournisseur
            .Include(r => r.LigneRetourMarchandiseFournisseur)
            .FirstOrDefaultAsync(r => r.Num == command.Num, cancellationToken);

        if (retour is null)
        {
            _logger.LogEntityNotFound(nameof(RetourMarchandiseFournisseur), command.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        if (retour.Statut == DocumentStatus.Valid)
        {
            return Result.Fail("Le document est validé et ne peut plus être modifié.");
        }

        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        // Remove all existing lines
        _context.LigneRetourMarchandiseFournisseur.RemoveRange(retour.LigneRetourMarchandiseFournisseur);

        // Create new lines
        var newLines = command.Lines.Select(line =>
            LigneRetourMarchandiseFournisseur.CreateRetourLine(
                retourMarchandiseFournisseurId: retour.Id,
                productRef: line.ProductRef,
                designationLigne: line.Description,
                quantity: line.Quantity,
                unitPrice: line.UnitPrice,
                discount: line.Discount,
                tax: line.Tax
            )
        ).ToList();

        // Calculate totals from new lines
        var totHTva = DecimalHelper.RoundAmount(newLines.Sum(l => l.TotHt));
        var totTva = DecimalHelper.RoundAmount(newLines.Sum(l => l.TotTtc - l.TotHt));
        var netPayer = DecimalHelper.RoundAmount(newLines.Sum(l => l.TotTtc));

        // Update the retour properties with calculated totals
        retour.UpdateRetourMarchandiseFournisseur(
            num: command.Num,
            date: command.Date,
            idFournisseur: command.IdFournisseur,
            accountingYearId: activeAccountingYear.Id,
            totHTva: totHTva,
            totTva: totTva,
            netPayer: netPayer
        );

        // Add new lines to the context explicitly
        await _context.LigneRetourMarchandiseFournisseur.AddRangeAsync(newLines, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated retour marchandise fournisseur {Num} with {LineCount} lines", command.Num, newLines.Count);
        _logger.LogEntityUpdated(nameof(RetourMarchandiseFournisseur), command.Num);

        return Result.Ok();
    }
}

