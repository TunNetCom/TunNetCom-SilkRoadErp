using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Linq;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;
using TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.DeliveryNotes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.Invoices;

public partial class ManageInvoices
{
    // Edit invoice date (opens dialog and calls API; only draft invoices)
    async Task EditInvoiceDate(int invoiceNumber)
    {
        var invoice = _invoicesWithDeliveryNotes.FirstOrDefault(i => i.Invoice.Number == invoiceNumber)?.Invoice;
        if (invoice is null || invoice.Statut != (int)DocumentStatus.Draft)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = Localizer["warning"],
                Detail = Localizer["invoice_date_edit_draft_only"]
            });
            return;
        }

        var initial = invoice.Date;

        var result = await DialogService.OpenAsync<CreateInvoiceDialog>(
            Localizer["edit_invoice_date"],
            new Dictionary<string, object>
            {
                { "InitialDate", initial },
                { "DescriptionResourceKey", "edit_invoice_date_prompt" }
            },
            new DialogOptions() { Width = "400px", Resizable = false, Draggable = true });

        if (result is DateTime selectedDate)
        {
            try
            {
                var request = new UpdateInvoiceDateRequest { Date = selectedDate };
                var res = await invoicesService.UpdateInvoiceDateAsync(invoiceNumber, request, _cancellationTokenSource.Token);
                if (res.IsSuccess)
                {
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = Localizer["success"], Detail = Localizer["invoice_date_updated"] });
                    if (InvoiceGrid != null)
                    {
                        await InvoiceGrid.Reload();
                    }
                    else
                    {
                        await LoadInvoicesForCustomers();
                    }
                }
                else
                {
                    var detail = res.Errors.FirstOrDefault()?.Message ?? Localizer["Unexpected_Error"];
                    if (string.Equals(detail, "invoice_date_only_draft", StringComparison.Ordinal))
                    {
                        detail = Localizer["invoice_date_edit_draft_only"];
                    }

                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = Localizer["error"], Detail = detail });
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = Localizer["error"], Detail = ex.Message });
            }
        }
    }
}
