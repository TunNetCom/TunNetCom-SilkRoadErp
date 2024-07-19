using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Threading;
using TunNetCom.SilkRoadErp.Sales.Api.Contracts.Providers;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.UpdateProvider;
public class UpdateProviderCommandHandler(SalesContext _context)
: IRequestHandler<UpdateProviderCommand, Result>
{ 
    public async Task<Result> Handle(UpdateProviderCommand updateProviderCommand, CancellationToken cancellationToken)
    {
        var providerToUpdate = await _context.Fournisseur.FindAsync(updateProviderCommand.Id);

        if (providerToUpdate is null)
        {
            return Result.Fail(EntityNotFound.Error);
        }
        var isProviderNameExist = await _context.Fournisseur.AnyAsync(
    provider => provider.Nom == updateProviderCommand.Nom
    && provider.Id != updateProviderCommand.Id,
    cancellationToken);

        if (isProviderNameExist)
        {
            return Result.Fail("Provider_name_exist");
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
        return Result.Ok();
    }
}