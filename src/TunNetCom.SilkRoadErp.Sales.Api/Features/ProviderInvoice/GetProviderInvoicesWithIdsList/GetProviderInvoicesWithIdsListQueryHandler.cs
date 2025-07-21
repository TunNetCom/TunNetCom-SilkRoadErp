using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Views;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetProviderInvoicesWithIdsList
{
    public class GetProviderInvoicesWithIdsListQueryHandler
        : IRequestHandler<GetProviderInvoicesWithIdsListQuery, List<ProviderInvoiceResponse>>
    {
        private readonly SalesContext _context;
        private readonly ILogger<GetProviderInvoicesWithIdsListQueryHandler> _logger;

        public GetProviderInvoicesWithIdsListQueryHandler(
            SalesContext context,
            ILogger<GetProviderInvoicesWithIdsListQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ProviderInvoiceResponse>> Handle(
            GetProviderInvoicesWithIdsListQuery request,
            CancellationToken cancellationToken)
        {
            var invoices = await _context.ProviderInvoiceView
                .AsNoTracking()
                .Where(f => request.InvoicesIds.Contains(f.Num))
                .Select(f => new ProviderInvoiceResponse
                {
                    Num = f.Num,
                    ProviderId = f.ProviderId,
                    Date = f.Date,
                    TotTTC = f.TotalTTC,
                    TotHTva = f.TotalHT,
                    TotTva = f.TotalTTC - f.TotalHT
                })
                .ToListAsync(cancellationToken);

            if (!invoices.Any())
            {
                _logger.LogInformation("No invoices found for the given IDs: {Ids}", string.Join(", ", request.InvoicesIds));
            }

            return invoices;
        }
    }
}
