using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetFullAvoirFournisseur;

public class GetFullAvoirFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetFullAvoirFournisseurQueryHandler> _logger)
    : IRequestHandler<GetFullAvoirFournisseurQuery, Result<FullAvoirFournisseurResponse>>
{
    public async Task<Result<FullAvoirFournisseurResponse>> Handle(GetFullAvoirFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching full AvoirFournisseur with Id {Id}", query.Id);

        var avoirFournisseur = await _context.AvoirFournisseur
            .AsNoTracking()
            .Where(a => a.Id == query.Id)
            .Include(a => a.Fournisseur)
            .Include(a => a.LigneAvoirFournisseur)
            .Select(a => new FullAvoirFournisseurResponse
            {
                Id = a.Id,
                Date = a.Date,
                FournisseurId = a.FournisseurId,
                NumFactureAvoirFournisseur = a.FactureAvoirFournisseurId,
                NumAvoirChezFournisseur = a.NumAvoirChezFournisseur,
                Fournisseur = a.Fournisseur != null ? new AvoirFournisseurProviderResponse
                {
                    Id = a.Fournisseur.Id,
                    Name = a.Fournisseur.Nom,
                    Phone = a.Fournisseur.Tel,
                    Fax = a.Fournisseur.Fax,
                    RegistrationNumber = a.Fournisseur.Matricule,
                    Code = a.Fournisseur.Code,
                    CategoryCode = a.Fournisseur.CodeCat,
                    SecondaryEstablishment = a.Fournisseur.EtbSec,
                    Mail = a.Fournisseur.Mail,
                    Adress = a.Fournisseur.Adresse
                } : null,
                Lines = a.LigneAvoirFournisseur.Select(l => new AvoirFournisseurLineResponse
                {
                    IdLi = l.IdLi,
                    RefProduit = l.RefProduit,
                    DesignationLi = l.DesignationLi,
                    QteLi = l.QteLi,
                    PrixHt = l.PrixHt,
                    Remise = l.Remise,
                    TotHt = l.TotHt,
                    Tva = l.Tva,
                    TotTtc = l.TotTtc
                }).ToList(),
                TotalExcludingTaxAmount = a.LigneAvoirFournisseur.Sum(l => l.TotHt),
                TotalVATAmount = a.LigneAvoirFournisseur.Sum(l => l.TotTtc - l.TotHt),
                TotalIncludingTaxAmount = a.LigneAvoirFournisseur.Sum(l => l.TotTtc)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (avoirFournisseur == null)
        {
            _logger.LogWarning("AvoirFournisseur with Id {Id} not found", query.Id);
            return Result.Fail("avoir_fournisseur_not_found");
        }

        _logger.LogInformation("Full AvoirFournisseur with Id {Id} fetched successfully", query.Id);
        return Result.Ok(avoirFournisseur);
    }
}

