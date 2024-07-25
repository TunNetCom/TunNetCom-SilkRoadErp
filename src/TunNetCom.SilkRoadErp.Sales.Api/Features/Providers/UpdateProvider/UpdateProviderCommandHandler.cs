namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.UpdateProvider;
public class UpdateProviderCommandHandler(SalesContext _context, ILogger<UpdateProviderCommandHandler> _logger)
: IRequestHandler<UpdateProviderCommand, Result>
{ 
    public async Task<Result> Handle(UpdateProviderCommand updateProviderCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(Fournisseur), updateProviderCommand.Id);

       var providerToUpdate = await _context.Fournisseur.FindAsync(updateProviderCommand.Id);

        if (providerToUpdate is null)
        {
            _logger.LogEntityNotFound(nameof(Fournisseur), updateProviderCommand.Id);
            return Result.Fail(EntityNotFound.Error);
        }

       var isProviderNameExist = await _context.Fournisseur.AnyAsync(provider => 
       (provider.Nom == updateProviderCommand.Nom) && (provider.Id != updateProviderCommand.Id), cancellationToken);

        if (isProviderNameExist)
        {
            return Result.Fail("provider_name_exists");
        }

        providerToUpdate.UpdateProvider(
            nom: updateProviderCommand.Nom,
            tel: updateProviderCommand.Tel,
            fax: updateProviderCommand.Fax,
            matricule: updateProviderCommand.Matricule,
            code: updateProviderCommand.Code,
            codeCat: updateProviderCommand.CodeCat,
            etbSec: updateProviderCommand.EtbSec,
            mail: updateProviderCommand.Mail,
            mailDeux: updateProviderCommand.MailDeux,
            constructeur: updateProviderCommand.Constructeur,
            adresse: updateProviderCommand.Adresse);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(Fournisseur), updateProviderCommand.Id);
        return Result.Ok();
    }
}