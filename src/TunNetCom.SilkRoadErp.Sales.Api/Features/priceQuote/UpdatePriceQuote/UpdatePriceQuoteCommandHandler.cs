namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.UpdatePriceQuote;

public class UpdatePriceQuoteCommandHandler : IRequestHandler<UpdatePriceQuoteCommand, Result>
{
    private readonly SalesContext _context;
    private readonly ILogger<UpdatePriceQuoteCommandHandler> _logger;

    public UpdatePriceQuoteCommandHandler(
        SalesContext context,
        ILogger<UpdatePriceQuoteCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdatePriceQuoteCommand updatePriceQuoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(Devis), updatePriceQuoteCommand.Num);

        // Chercher le devis à mettre à jour avec ses lignes
        var quotationToUpdate = await _context.Devis
            .Include(d => d.LigneDevis)
            .FirstOrDefaultAsync(d => d.Num == updatePriceQuoteCommand.Num, cancellationToken);

        if (quotationToUpdate is null)
        {
            _logger.LogEntityNotFound(nameof(Devis), updatePriceQuoteCommand.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        // Mettre à jour les propriétés
        quotationToUpdate.UpdateDevis(
            num: updatePriceQuoteCommand.Num,
            idClient: updatePriceQuoteCommand.IdClient,
            date: updatePriceQuoteCommand.Date,
            totHTva: updatePriceQuoteCommand.TotHTva,
            totTva: updatePriceQuoteCommand.TotTva,
            totTtc: updatePriceQuoteCommand.TotTtc);

        // Supprimer les anciennes lignes
        _context.LigneDevis.RemoveRange(quotationToUpdate.LigneDevis);

        // Ajouter les nouvelles lignes
        foreach (var quotationLine in updatePriceQuoteCommand.QuotationLines)
        {
            var ligneDevis = new LigneDevis
            {
                RefProduit = quotationLine.RefProduit,
                DesignationLi = quotationLine.DesignationLi,
                QteLi = quotationLine.QteLi,
                PrixHt = quotationLine.PrixHt,
                Remise = quotationLine.Remise,
                TotHt = quotationLine.TotHt,
                Tva = quotationLine.Tva,
                TotTtc = quotationLine.TotTtc,
                DevisId = quotationToUpdate.Num,
                NumDevisNavigation = quotationToUpdate
            };

            quotationToUpdate.LigneDevis.Add(ligneDevis);
        }

        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(Devis), updatePriceQuoteCommand.Num);

        return Result.Ok();
    }
}
