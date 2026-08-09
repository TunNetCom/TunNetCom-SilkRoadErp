using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class AuditLogTest
{
    [Fact]
    public void Create_WithAllValues_ShouldSetProperties()
    {
        var timestampBefore = DateTime.UtcNow.AddSeconds(-1);

        var log = AuditLog.Create(
            entityName: "Client",
            entityId: "42",
            action: AuditAction.Updated,
            userId: 7,
            username: "john",
            oldValues: "{\"Name\":\"Old\"}",
            newValues: "{\"Name\":\"New\"}",
            changedProperties: "[\"Name\"]");

        log.EntityName.Should().Be("Client");
        log.EntityId.Should().Be("42");
        log.Action.Should().Be(AuditAction.Updated);
        log.UserId.Should().Be(7);
        log.Username.Should().Be("john");
        log.OldValues.Should().Be("{\"Name\":\"Old\"}");
        log.NewValues.Should().Be("{\"Name\":\"New\"}");
        log.ChangedProperties.Should().Be("[\"Name\"]");
        log.Timestamp.Should().BeAfter(timestampBefore);
        log.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Theory]
    [InlineData(AuditAction.Created)]
    [InlineData(AuditAction.Updated)]
    [InlineData(AuditAction.Deleted)]
    public void Create_WithEachAction_ShouldSetAction(AuditAction action)
    {
        var log = AuditLog.Create(
            entityName: "Produit",
            entityId: "1",
            action: action,
            userId: null,
            username: null,
            oldValues: null,
            newValues: null,
            changedProperties: null);

        log.Action.Should().Be(action);
    }

    [Fact]
    public void Create_WhenUsernameNull_ShouldDefaultToSystem()
    {
        var log = AuditLog.Create(
            entityName: "Client",
            entityId: "1",
            action: AuditAction.Created,
            userId: null,
            username: null,
            oldValues: null,
            newValues: null,
            changedProperties: null);

        log.Username.Should().Be("System");
    }

    [Fact]
    public void Create_WhenOptionalValuesNull_ShouldKeepThemNull()
    {
        var log = AuditLog.Create(
            entityName: "Client",
            entityId: "1",
            action: AuditAction.Created,
            userId: null,
            username: "user",
            oldValues: null,
            newValues: null,
            changedProperties: null);

        log.UserId.Should().BeNull();
        log.OldValues.Should().BeNull();
        log.NewValues.Should().BeNull();
        log.ChangedProperties.Should().BeNull();
    }

    [Fact]
    public void SetId_ShouldUpdateId()
    {
        var log = AuditLog.Create(
            entityName: "Client",
            entityId: "1",
            action: AuditAction.Created,
            userId: null,
            username: null,
            oldValues: null,
            newValues: null,
            changedProperties: null);

        log.Id.Should().Be(0);
        log.SetId(123);
        log.Id.Should().Be(123);
    }
}
