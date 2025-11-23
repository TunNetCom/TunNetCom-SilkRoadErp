using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

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
        
        return builder.GetEdmModel();
    }
}

