using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.GetRetourMarchandiseFournisseur;

public class GetRetourMarchandiseFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetRetourMarchandiseFournisseurQueryHandler> _logger)
    : IRequestHandler<GetRetourMarchandiseFournisseurQuery, Result<RetourMarchandiseFournisseurResponse>>
{
    public async Task<Result<RetourMarchandiseFournisseurResponse>> Handle(
        GetRetourMarchandiseFournisseurQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(RetourMarchandiseFournisseur), query.Num);
        
        // Load data first to avoid SQL conversion issues with Statut (string -> enum -> int)
        var retour = await _context.RetourMarchandiseFournisseur
            .Include(r => r.IdFournisseurNavigation)
            .Include(r => r.LigneRetourMarchandiseFournisseur)
            .FirstOrDefaultAsync(r => r.Num == query.Num, cancellationToken);
            
        if (retour is null)
        {
            _logger.LogEntityNotFound(nameof(RetourMarchandiseFournisseur), query.Num);
            return Result.Fail(EntityNotFound.Error());
        }
        
        // Map to DTO in memory to avoid SQL conversion issues
        var retourResponse = new RetourMarchandiseFournisseurResponse
        {
            Id = retour.Id,
            Num = retour.Num,
            Date = retour.Date,
            IdFournisseur = retour.IdFournisseur,
            NomFournisseur = retour.IdFournisseurNavigation?.Nom,
            TotHTva = retour.TotHTva,
            TotTva = retour.TotTva,
            NetPayer = retour.NetPayer,
            Statut = (int)retour.Statut,
            StatutLibelle = retour.Statut == DocumentStatus.Valid ? "Validé" : "Brouillon",
            Lines = retour.LigneRetourMarchandiseFournisseur.Select(l => new LigneRetourMarchandiseFournisseurResponse
            {
                IdLigne = l.IdLigne,
                RefProduit = l.RefProduit,
                DesignationLi = l.DesignationLi,
                QteLi = l.QteLi,
                PrixHt = l.PrixHt,
                Remise = l.Remise,
                TotHt = l.TotHt,
                Tva = l.Tva,
                TotTtc = l.TotTtc
            }).ToList()
        };
        
        _logger.LogEntityFetchedById(nameof(RetourMarchandiseFournisseur), query.Num);
        return retourResponse;
    }
}

