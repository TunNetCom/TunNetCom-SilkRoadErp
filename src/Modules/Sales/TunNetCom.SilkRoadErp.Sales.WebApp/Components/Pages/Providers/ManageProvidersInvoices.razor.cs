using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.DeliveryNotes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.Providers;

public partial class ManageProvidersInvoices
{
    private bool ProviderInvoiceIsDraft(int invoiceNum)
    {
        var inv = _invoicesWithReceiptNotes.FirstOrDefault(i => i.Invoice.Num == invoiceNum)?.Invoice;
        return inv != null && inv.Statut == (int)DocumentStatus.Draft;
    }

    /// <summary>True when no invoice is selected yet, or the selected invoice is still draft (receipt-note attach flow).</summary>
    private bool CanSelectReceiptNotesToAttach =>
        SelectedInvoiceId <= _minValueId || ProviderInvoiceIsDraft(SelectedInvoiceId);

    private bool IsSelectedProviderInvoiceDraft =>
        SelectedInvoiceId > _minValueId && ProviderInvoiceIsDraft(SelectedInvoiceId);

    private void NotifyInvoiceEditingRequiresDraft()
    {
        NotificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Warning,
            Summary = Localizer["warning"],
            Detail = Localizer["invoice_editing_requires_draft"]
        });
    }

    async Task EditProviderInvoiceDate(int invoiceNumber)
    {
        var invoice = _invoicesWithReceiptNotes.FirstOrDefault(i => i.Invoice.Num == invoiceNumber)?.Invoice;
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

        var result = await DialogService.OpenAsync<CreateInvoiceDialog>(
            Localizer["edit_invoice_date"],
            new Dictionary<string, object>
            {
                { "InitialDate", invoice.Date },
                { "DescriptionResourceKey", "edit_invoice_date_prompt" }
            },
            new DialogOptions { Width = "400px", Resizable = false, Draggable = true });

        if (result is DateTime selectedDate)
        {
            try
            {
                var request = new UpdateProviderInvoiceDateRequest { Date = selectedDate };
                var res = await providerInvoiceService.UpdateProviderInvoiceDateAsync(invoiceNumber, request, _cancellationTokenSource.Token);
                if (res.IsSuccess)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = Localizer["success"],
                        Detail = Localizer["invoice_date_updated"]
                    });
                    if (ProviderInvoiceGrid != null)
                    {
                        await ProviderInvoiceGrid.Reload();
                    }
                    else
                    {
                        await LoadInvoices(SelectedProviderId);
                    }
                }
                else
                {
                    var detail = res.Errors.FirstOrDefault()?.Message ?? Localizer["Unexpected_Error"];
                    if (string.Equals(detail, "invoice_date_only_draft", StringComparison.Ordinal))
                    {
                        detail = Localizer["invoice_date_edit_draft_only"];
                    }

                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = Localizer["error"],
                        Detail = detail
                    });
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = Localizer["error"],
                    Detail = ex.Message
                });
            }
        }
    }
}
