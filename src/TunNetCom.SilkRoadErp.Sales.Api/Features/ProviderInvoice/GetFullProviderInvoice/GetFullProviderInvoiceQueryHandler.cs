using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetFullInvoiceById;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetFullProviderInvoice;

public class GetFullProviderInvoiceQueryHandler(
    SalesContext _context,
    ILogger<GetFullProviderInvoiceQueryHandler> _logger)
    : IRequestHandler<GetFullProviderInvoiceQuery, Result<FullProviderInvoiceResponse>>
{
    public async Task<Result<FullProviderInvoiceResponse>> Handle(
        GetFullProviderInvoiceQuery query,
        CancellationToken cancellationToken)
    {
        
        FullProviderInvoiceResponse? invoice = await GetFullInvoiceByIdAsync(query, cancellationToken);

        if (invoice is null)
        {
            _logger.LogEntityNotFound(
                nameof(FactureFournisseur),
                query.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        _logger.LogEntityFetchedById(
            nameof(FactureFournisseur),
            query.Id);

        return invoice;
    }

    private async Task<FullProviderInvoiceResponse?> GetFullInvoiceByIdAsync(
        GetFullProviderInvoiceQuery query,
        CancellationToken cancellationToken)
    {
        var providerInvoice = await _context.FactureFournisseur
            .AsNoTracking()
            .Where(f => f.Num == query.Id)
            .Include(f => f.IdFournisseurNavigation)
            .Include(f => f.BonDeReception)
                .ThenInclude(bl => bl.LigneBonReception)
            .Select(f => new FullProviderInvoiceResponse
            {
                Num = f.Num,
                Date = f.Date,
                ProviderId = f.IdFournisseur,
                Provider = f.IdFournisseurNavigation != null ? new ProviderInfos
                {
                    Id = f.IdFournisseurNavigation.Id,
                    Nom = f.IdFournisseurNavigation.Nom,
                    Tel = f.IdFournisseurNavigation.Tel,
                    Adresse = f.IdFournisseurNavigation.Adresse,
                    Matricule = f.IdFournisseurNavigation.Matricule,
                    Code = f.IdFournisseurNavigation.Code,
                    CodeCat = f.IdFournisseurNavigation.CodeCat,
                    EtbSec = f.IdFournisseurNavigation.EtbSec,
                    Mail = f.IdFournisseurNavigation.Mail
                } : null!,
                ReceiptNotes = f.BonDeReception.Select(bl => new FullProviderInvoiceReceiptNotes
                {
                    Num = bl.Num,
                    Date = bl.Date,
                    ProviderId = bl.IdFournisseur,
                    Lines = bl.LigneBonReception.Select(li => new ReceiptNotesLine
                    {
                        IdLi = li.IdLigne,
                        RefProduit = li.RefProduit,
                        DesignationLi = li.DesignationLi,
                        QteLi = li.QteLi,
                        PrixHt = li.PrixHt,
                        Remise = li.Remise,
                        TotHt = li.TotHt,
                        Tva = li.Tva,
                        TotTtc = li.TotTtc
                    }).ToList()
                }).ToList()
            }
            ).FirstOrDefaultAsync(cancellationToken);
        
        providerInvoice.ReceiptNotes.ForEach(bl =>
        {
            bl.TotHTva = bl.Lines.Sum(li => li.TotHt);
            bl.NetPayer = bl.Lines.Sum(li => li.TotTtc);
            bl.TotTva = bl.NetPayer - bl.TotHTva;
        });

        return providerInvoice;
        
    }
}
