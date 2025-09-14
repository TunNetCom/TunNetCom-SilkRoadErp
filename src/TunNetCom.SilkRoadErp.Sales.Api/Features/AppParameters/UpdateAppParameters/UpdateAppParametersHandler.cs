
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
        var appParametersToUpdate = await _context.Systeme.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        if (appParametersToUpdate is null)
        {
            _logger.LogEntityNotFound(nameof(Systeme), 1);
            return Result.Fail(EntityNotFound.Error());
        }

        // Update the app parameters with the provided values
        appParametersToUpdate.UpdateSysteme(
            nomSociete: updateAppParametersCommand.NomSociete,
            timbre: (decimal)updateAppParametersCommand.Timbre,
            adresse: updateAppParametersCommand.Adresse,
            tel: updateAppParametersCommand.Tel,
            fax: updateAppParametersCommand.Fax,
            email: updateAppParametersCommand.Email,
            matriculeFiscale: updateAppParametersCommand.MatriculeFiscale,
            codeTva: updateAppParametersCommand.CodeTva,
            codeCategorie: updateAppParametersCommand.CodeCategorie,
            etbSecondaire: updateAppParametersCommand.EtbSecondaire,
            pourcentageFodec: (decimal)updateAppParametersCommand.PourcentageFodec,
            adresseRetenu: updateAppParametersCommand.AdresseRetenu,
            pourcentageRetenu: (double)updateAppParametersCommand.PourcentageRetenu);

        _ = await _context.SaveChangesAsync(cancellationToken);
        _logger.LogEntityUpdated(nameof(Systeme), 1);

        return Result.Ok();
    }
}
