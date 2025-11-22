using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Views;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class ProviderInvoiceViewConfiguration : IEntityTypeConfiguration<ProviderInvoiceView>
{
    public void Configure(EntityTypeBuilder<ProviderInvoiceView> builder)
    {
        builder.ToView("ProviderInvoiceView", "dbo");
        builder.HasNoKey();
    }
}


