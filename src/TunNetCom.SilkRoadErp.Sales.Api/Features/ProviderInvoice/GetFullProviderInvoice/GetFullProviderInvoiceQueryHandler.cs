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
                ProviderInvoiceNumber = f.Num,
                Date = f.Date,
                ProviderId = f.IdFournisseur,
                Provider = f.IdFournisseurNavigation != null ? new ProviderInfos
                {
                    Id = f.IdFournisseurNavigation.Id,
                    Name = f.IdFournisseurNavigation.Nom,
                    Phone = f.IdFournisseurNavigation.Tel,
                    Adress = f.IdFournisseurNavigation.Adresse,
                    RegistrationNumber = f.IdFournisseurNavigation.Matricule,
                    Code = f.IdFournisseurNavigation.Code,
                    CategoryCode = f.IdFournisseurNavigation.CodeCat,
                    SecondaryEstablishment = f.IdFournisseurNavigation.EtbSec,
                    Mail = f.IdFournisseurNavigation.Mail
                } : null!,
                ReceiptNotes = f.BonDeReception.Select(bl => new FullProviderInvoiceReceiptNotes
                {
                    ReceiptNoteNumber = bl.Num,
                    Date = bl.Date,
                    ProviderId = bl.IdFournisseur,
                    Lines = bl.LigneBonReception.Select(li => new ReceiptNotesLine
                    {
                        LineId = li.IdLigne,
                        ProductReference = li.RefProduit,
                        ItemDescription = li.DesignationLi,
                        ItemQuantity = li.QteLi,
                        UnitPriceExcludingTax = li.PrixHt,
                        Discount = li.Remise,
                        TotalExcludingTax = li.TotHt,
                        VatRate = li.Tva,
                        TotalIncludingTax = li.TotTtc
                    }).ToList()
                }).ToList()
            }
            ).FirstOrDefaultAsync(cancellationToken);
        
        providerInvoice.ReceiptNotes.ForEach(bl =>
        {
            bl.TotalExcludingVat = bl.Lines.Sum(li => li.TotalExcludingTax);
            bl.NetToPay = bl.Lines.Sum(li => li.TotalIncludingTax);
            bl.TotalVat = bl.NetToPay - bl.TotalExcludingVat;
        });

        return providerInvoice;
        
    }
}
