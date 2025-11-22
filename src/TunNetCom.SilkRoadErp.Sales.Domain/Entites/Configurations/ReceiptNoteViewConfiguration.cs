using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Views;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class ReceiptNoteViewConfiguration : IEntityTypeConfiguration<ReceiptNoteView>
{
    public void Configure(EntityTypeBuilder<ReceiptNoteView> builder)
    {
        builder.ToView("ReceiptNoteView", "dbo");
        builder.HasNoKey();
    }
}


