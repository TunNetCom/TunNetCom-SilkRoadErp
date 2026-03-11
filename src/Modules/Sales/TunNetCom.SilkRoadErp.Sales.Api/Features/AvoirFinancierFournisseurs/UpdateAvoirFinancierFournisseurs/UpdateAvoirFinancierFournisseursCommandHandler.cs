namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.UpdateAvoirFinancierFournisseurs;

public class UpdateAvoirFinancierFournisseursCommandHandler(
    SalesContext _context,
    ILogger<UpdateAvoirFinancierFournisseursCommandHandler> _logger)
    : IRequestHandler<UpdateAvoirFinancierFournisseursCommand, Result>
{
    public async Task<Result> Handle(UpdateAvoirFinancierFournisseursCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateAvoirFinancierFournisseursCommand called with Num {Num}", command.Num);

        var avoirFinancier = await _context.AvoirFinancierFournisseurs
            .FirstOrDefaultAsync(a => a.Num == command.Num, cancellationToken);

        if (avoirFinancier == null)
        {
            _logger.LogEntityNotFound(nameof(Domain.Entites.AvoirFinancierFournisseurs), command.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        avoirFinancier.NumSurPage = command.NumSurPage;
        avoirFinancier.Date = command.Date;
        avoirFinancier.Description = command.Description;
        avoirFinancier.TotTtc = command.TotTtc;

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("AvoirFinancierFournisseurs with Num {Num} updated successfully", avoirFinancier.Num);
        return Result.Ok();
    }
}

