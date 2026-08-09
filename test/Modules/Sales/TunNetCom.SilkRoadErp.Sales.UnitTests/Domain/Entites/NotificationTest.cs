using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class NotificationTest
{
    [Fact]
    public void CreateNotification_ShouldSetProperties()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var notification = Notification.CreateNotification(
            type: NotificationType.LowStock,
            title: "Stock faible",
            message: "Le produit PRD-001 est en stock faible",
            relatedEntityId: 5,
            relatedEntityType: "Produit",
            userId: 2);

        notification.Type.Should().Be(NotificationType.LowStock);
        notification.Title.Should().Be("Stock faible");
        notification.Message.Should().Be("Le produit PRD-001 est en stock faible");
        notification.RelatedEntityId.Should().Be(5);
        notification.RelatedEntityType.Should().Be("Produit");
        notification.UserId.Should().Be(2);
        notification.IsRead.Should().BeFalse();
        notification.ReadAt.Should().BeNull();
        notification.CreatedAt.Should().BeAfter(before);
        notification.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Theory]
    [InlineData(NotificationType.UnpaidClient)]
    [InlineData(NotificationType.LowStock)]
    [InlineData(NotificationType.SupplierReturn)]
    public void CreateNotification_WithEachType_ShouldSetType(NotificationType type)
    {
        var notification = Notification.CreateNotification(
            type: type,
            title: "T",
            message: "M");

        notification.Type.Should().Be(type);
        notification.RelatedEntityId.Should().BeNull();
        notification.RelatedEntityType.Should().BeNull();
        notification.UserId.Should().BeNull();
    }

    [Fact]
    public void MarkAsRead_WhenUnread_ShouldSetRead()
    {
        var notification = Notification.CreateNotification(
            NotificationType.UnpaidClient, "T", "M");
        var before = DateTime.UtcNow.AddSeconds(-1);

        notification.MarkAsRead();

        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().BeAfter(before);
    }

    [Fact]
    public void MarkAsRead_WhenAlreadyRead_ShouldNotChangeReadAt()
    {
        var notification = Notification.CreateNotification(
            NotificationType.UnpaidClient, "T", "M");
        notification.MarkAsRead();
        var originalReadAt = notification.ReadAt;

        notification.MarkAsRead();

        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().Be(originalReadAt);
    }
}
