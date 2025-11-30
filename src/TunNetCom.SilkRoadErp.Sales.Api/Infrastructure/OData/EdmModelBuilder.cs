using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.OData;

public static class EdmModelBuilder
{
    public static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        
        // Configure InvoiceBaseInfo entity set
        var invoicesEntitySet = builder.EntitySet<InvoiceBaseInfo>("InvoiceBaseInfos");
        invoicesEntitySet.EntityType.HasKey(i => i.Number);
        invoicesEntitySet.EntityType.Property(i => i.Number).IsRequired();
        invoicesEntitySet.EntityType.Property(i => i.Date).IsRequired();
        invoicesEntitySet.EntityType.Property(i => i.CustomerId).IsRequired();
        invoicesEntitySet.EntityType.Property(i => i.CustomerName);
        invoicesEntitySet.EntityType.Property(i => i.NetAmount).IsRequired();
        invoicesEntitySet.EntityType.Property(i => i.VatAmount).IsRequired();
        
        // Configure GetDeliveryNoteBaseInfos entity set
        var deliveryNotesEntitySet = builder.EntitySet<GetDeliveryNoteBaseInfos>("DeliveryNoteBaseInfos");
        deliveryNotesEntitySet.EntityType.HasKey(d => d.Number);
        deliveryNotesEntitySet.EntityType.Property(d => d.Number).IsRequired();
        deliveryNotesEntitySet.EntityType.Property(d => d.Date).IsRequired();
        deliveryNotesEntitySet.EntityType.Property(d => d.CustomerId);
        deliveryNotesEntitySet.EntityType.Property(d => d.CustomerName);
        deliveryNotesEntitySet.EntityType.Property(d => d.NetAmount).IsRequired();
        deliveryNotesEntitySet.EntityType.Property(d => d.GrossAmount).IsRequired();
        deliveryNotesEntitySet.EntityType.Property(d => d.VatAmount).IsRequired();
        deliveryNotesEntitySet.EntityType.Property(d => d.NumFacture);
        
        // Configure ReceiptNoteBaseInfo entity set
        var receiptNotesEntitySet = builder.EntitySet<ReceiptNoteBaseInfo>("ReceiptNoteBaseInfos");
        receiptNotesEntitySet.EntityType.HasKey(r => r.Number);
        receiptNotesEntitySet.EntityType.Property(r => r.Number).IsRequired();
        receiptNotesEntitySet.EntityType.Property(r => r.Date).IsRequired();
        receiptNotesEntitySet.EntityType.Property(r => r.ProviderId).IsRequired();
        receiptNotesEntitySet.EntityType.Property(r => r.ProviderName);
        receiptNotesEntitySet.EntityType.Property(r => r.NetAmount).IsRequired();
        receiptNotesEntitySet.EntityType.Property(r => r.VatAmount).IsRequired();
        
        // Configure QuotationBaseInfo entity set
        var quotationsEntitySet = builder.EntitySet<QuotationBaseInfo>("QuotationBaseInfos");
        quotationsEntitySet.EntityType.HasKey(q => q.Number);
        quotationsEntitySet.EntityType.Property(q => q.Number).IsRequired();
        quotationsEntitySet.EntityType.Property(q => q.Date).IsRequired();
        quotationsEntitySet.EntityType.Property(q => q.CustomerId).IsRequired();
        quotationsEntitySet.EntityType.Property(q => q.CustomerName);
        quotationsEntitySet.EntityType.Property(q => q.TotalTtc).IsRequired();
        
        // Configure ProviderInvoiceBaseInfo entity set
        var providerInvoicesEntitySet = builder.EntitySet<ProviderInvoiceBaseInfo>("ProviderInvoiceBaseInfos");
        providerInvoicesEntitySet.EntityType.HasKey(p => p.Number);
        providerInvoicesEntitySet.EntityType.Property(p => p.Number).IsRequired();
        providerInvoicesEntitySet.EntityType.Property(p => p.Date).IsRequired();
        providerInvoicesEntitySet.EntityType.Property(p => p.ProviderId).IsRequired();
        providerInvoicesEntitySet.EntityType.Property(p => p.NetAmount).IsRequired();
        providerInvoicesEntitySet.EntityType.Property(p => p.VatAmount).IsRequired();
        
        // Configure InstallationTechnicianResponse entity set
        var techniciansEntitySet = builder.EntitySet<InstallationTechnicianResponse>("InstallationTechnicianBaseInfos");
        techniciansEntitySet.EntityType.HasKey(t => t.Id);
        techniciansEntitySet.EntityType.Property(t => t.Id).IsRequired();
        techniciansEntitySet.EntityType.Property(t => t.Nom).IsRequired();
        techniciansEntitySet.EntityType.Property(t => t.Tel);
        techniciansEntitySet.EntityType.Property(t => t.Tel2);
        techniciansEntitySet.EntityType.Property(t => t.Tel3);
        techniciansEntitySet.EntityType.Property(t => t.Email);
        techniciansEntitySet.EntityType.Property(t => t.Description);
        techniciansEntitySet.EntityType.Property(t => t.Photo);
        
        return builder.GetEdmModel();
    }
}

