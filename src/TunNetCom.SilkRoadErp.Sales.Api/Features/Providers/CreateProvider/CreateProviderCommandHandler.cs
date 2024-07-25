namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.CreateProvider;
public class CreateProviderCommandHandler(SalesContext _context, ILogger<CreateProviderCommandHandler> _logger)
: IRequestHandler<CreateProviderCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateProviderCommand createProviderCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(Fournisseur), createProviderCommand);

        var isProviderNameExist = await _context.Fournisseur.AnyAsync(provider => provider.Nom == createProviderCommand.Nom, cancellationToken);

        if (isProviderNameExist)
        {
            return Result.Fail("provider_name_exists");
        }

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

        _logger.LogEntityCreatedSuccessfully(nameof(Fournisseur), provider.Id);
        return provider.Id;
    }
}
