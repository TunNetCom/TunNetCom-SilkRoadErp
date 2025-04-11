using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetProviderInvoicesWithIdsList;

public class GetProviderInvoicesWithIdsListQueryHandler(
    SalesContext context,
    ILogger<GetProviderInvoicesWithIdsListQueryHandler> logger)
    : IRequestHandler<GetProviderInvoicesWithIdsListQuery, List<ProviderInvoiceResponse>>
{
    public async Task<List<ProviderInvoiceResponse>> Handle(
        GetProviderInvoicesWithIdsListQuery request,
        CancellationToken cancellationToken)
    {
        var invoiceQuery = context.ProviderInvoiceView
            .Where(f => request.InvoicesIds.Contains(f.Num)) // Assuming 'Id' exists; adjust if it's different
            .Select(f => new ProviderInvoiceResponse
            {
                Num = f.Num, // Convert int to string if Num in response is string
                ProviderId = f.ProviderId,
                Date = f.Date,
                TotTTC = f.TotalTTC,
                TotHTva = f.TotalHT,
                TotTva = f.TotalTTC - f.TotalHT // Compute TotTva since it's not a DB column
            })
            .ToList();

        if (!invoiceQuery.Any())
        {
            logger.LogInformation("No invoices found for IDs: {InvoiceIds}", string.Join(", ", request.InvoicesIds));
            return new List<ProviderInvoiceResponse>();
        }

        logger.LogInformation("Successfully retrieved {Count} invoices.", invoiceQuery.Count);
        return  invoiceQuery;
    }
}