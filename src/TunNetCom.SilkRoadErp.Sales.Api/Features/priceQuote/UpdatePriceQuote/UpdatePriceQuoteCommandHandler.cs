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

        // Chercher le devis à mettre à jour
        var quotationToUpdate = await _context.Devis.FindAsync(new object[] { updatePriceQuoteCommand.Num }, cancellationToken);

        if (quotationToUpdate is null)
        {
            _logger.LogEntityNotFound(nameof(Devis), updatePriceQuoteCommand.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        // Vérifier qu’aucun autre devis (différent) n’a le même numéro
        var isQuotationNumExist = await _context.Devis.AnyAsync(
            quo => quo.Num == updatePriceQuoteCommand.Num && quo.Num != quotationToUpdate.Num,
            cancellationToken);

        if (isQuotationNumExist)
        {
            return Result.Fail("quotation_num_exist");
        }

        // Mettre à jour les propriétés
        quotationToUpdate.UpdateDevis(
            num: updatePriceQuoteCommand.Num,
            idClient: updatePriceQuoteCommand.IdClient,
            date: updatePriceQuoteCommand.Date,
            totHTva: updatePriceQuoteCommand.TotHTva,
            totTva: updatePriceQuoteCommand.TotTva,
            totTtc: updatePriceQuoteCommand.TotTtc);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(Devis), updatePriceQuoteCommand.Num);

        return Result.Ok();
    }
}
