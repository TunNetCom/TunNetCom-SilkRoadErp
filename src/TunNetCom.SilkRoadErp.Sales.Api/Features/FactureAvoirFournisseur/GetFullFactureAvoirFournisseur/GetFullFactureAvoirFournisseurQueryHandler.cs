using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFullFactureAvoirFournisseur;

public class GetFullFactureAvoirFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetFullFactureAvoirFournisseurQueryHandler> _logger)
    : IRequestHandler<GetFullFactureAvoirFournisseurQuery, Result<FullFactureAvoirFournisseurResponse>>
{
    public async Task<Result<FullFactureAvoirFournisseurResponse>> Handle(GetFullFactureAvoirFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching full FactureAvoirFournisseur with Id {Id}", query.Id);

        var factureAvoirFournisseur = await _context.FactureAvoirFournisseur
            .AsNoTracking()
            .Where(f => f.Id == query.Id)
            .Include(f => f.IdFournisseurNavigation)
            .Include(f => f.AvoirFournisseur)
                .ThenInclude(a => a.LigneAvoirFournisseur)
            .Select(f => new FullFactureAvoirFournisseurResponse
            {
                Id = f.Id,
                NumFactureAvoirFourSurPage = f.NumFactureAvoirFourSurPage,
                Date = f.Date,
                IdFournisseur = f.IdFournisseur,
                NumFactureFournisseur = f.FactureFournisseurId,
                Fournisseur = new FactureAvoirFournisseurProviderResponse
                {
                    Id = f.IdFournisseurNavigation.Id,
                    Name = f.IdFournisseurNavigation.Nom,
                    Phone = f.IdFournisseurNavigation.Tel,
                    Fax = f.IdFournisseurNavigation.Fax,
                    RegistrationNumber = f.IdFournisseurNavigation.Matricule,
                    Code = f.IdFournisseurNavigation.Code,
                    CategoryCode = f.IdFournisseurNavigation.CodeCat,
                    SecondaryEstablishment = f.IdFournisseurNavigation.EtbSec,
                    Mail = f.IdFournisseurNavigation.Mail,
                    Adress = f.IdFournisseurNavigation.Adresse
                },
                AvoirFournisseurs = f.AvoirFournisseur.Select(a => new FactureAvoirFournisseurAvoirResponse
                {
                    Id = a.Id,
                    Date = a.Date,
                    NumAvoirChezFournisseur = a.NumAvoirChezFournisseur,
                    Lines = a.LigneAvoirFournisseur.Select(l => new FactureAvoirFournisseurAvoirLineResponse
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
                }).ToList(),
                TotalExcludingTaxAmount = f.AvoirFournisseur.Sum(a => a.LigneAvoirFournisseur.Sum(l => l.TotHt)),
                TotalVATAmount = f.AvoirFournisseur.Sum(a => a.LigneAvoirFournisseur.Sum(l => l.TotTtc - l.TotHt)),
                TotalIncludingTaxAmount = f.AvoirFournisseur.Sum(a => a.LigneAvoirFournisseur.Sum(l => l.TotTtc))
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (factureAvoirFournisseur == null)
        {
            _logger.LogWarning("FactureAvoirFournisseur with Id {Id} not found", query.Id);
            return Result.Fail("facture_avoir_fournisseur_not_found");
        }

        _logger.LogInformation("Full FactureAvoirFournisseur with Id {Id} fetched successfully", query.Id);
        return Result.Ok(factureAvoirFournisseur);
    }
}

