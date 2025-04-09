using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice; 

public interface IProviderInvoiceApiClient
{
    Task<GetProviderInvoicesWithSummary> GetProvidersInvoicesAsync(
        int idFournisseur,
        QueryStringParameters query,
        CancellationToken cancellationToken = default);
}
