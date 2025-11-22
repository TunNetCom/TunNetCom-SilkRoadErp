
namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.UpdateAppParameters;

public class UpdateAppParametersCommandHandler(
    SalesContext _context,
    ILogger<UpdateAppParametersCommandHandler> _logger)
    : IRequestHandler<UpdateAppParametersCommand, Result>
{
    // Handler for updating app parameters in the database
    public async Task<Result> Handle(
        UpdateAppParametersCommand updateAppParametersCommand,
        CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(Systeme), 1);

        // Check if the app parameters exist in the database
        var appParametersToUpdate = await _context.Systeme.FirstOrDefaultAsync(cancellationToken);

        if (appParametersToUpdate is null)
        {
            _logger.LogEntityNotFound(nameof(Systeme), 1);
            return Result.Fail(EntityNotFound.Error());
        }

        // Update the app parameters with the provided values
        appParametersToUpdate.UpdateSysteme(
            nomSociete: updateAppParametersCommand.NomSociete,
            timbre: updateAppParametersCommand.Timbre ?? appParametersToUpdate.Timbre,
            adresse: updateAppParametersCommand.Adresse ?? appParametersToUpdate.Adresse,
            tel: updateAppParametersCommand.Tel ?? appParametersToUpdate.Tel,
            fax: updateAppParametersCommand.Fax,
            email: updateAppParametersCommand.Email,
            matriculeFiscale: updateAppParametersCommand.MatriculeFiscale,
            codeTva: updateAppParametersCommand.CodeTva ?? appParametersToUpdate.CodeTva,
            codeCategorie: updateAppParametersCommand.CodeCategorie,
            etbSecondaire: updateAppParametersCommand.EtbSecondaire,
            pourcentageFodec: updateAppParametersCommand.PourcentageFodec ?? appParametersToUpdate.PourcentageFodec,
            adresseRetenu: updateAppParametersCommand.AdresseRetenu,
            pourcentageRetenu: updateAppParametersCommand.PourcentageRetenu ?? appParametersToUpdate.PourcentageRetenu,
            vatAmount: updateAppParametersCommand.VatAmount ?? appParametersToUpdate.VatAmount,
            discountPercentage: updateAppParametersCommand.DiscountPercentage ?? appParametersToUpdate.DiscountPercentage);

        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(Systeme), 1);

        return Result.Ok();
    }
}
