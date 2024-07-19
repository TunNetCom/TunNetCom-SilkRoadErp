using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.CreateProvider;
public class CreateProviderCommandHandler(SalesContext _context)
: IRequestHandler<CreateProviderCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateProviderCommand createProviderCommand, CancellationToken cancellationToken)
    {
        var provider = Fournisseur.CreateProvider(    
           createProviderCommand.Nom,
           createProviderCommand.Tel,
           createProviderCommand.Fax,
           createProviderCommand.Matricule,
           createProviderCommand.Code,
           createProviderCommand.CodeCat,
           createProviderCommand.EtbSec,
           createProviderCommand.Mail,
            createProviderCommand.MailDeux,
            createProviderCommand.Constructeur,
           createProviderCommand.Adresse

            );

        _context.Fournisseur.Add(provider);
        await _context.SaveChangesAsync(cancellationToken);
        return provider.Id;

    }

}
