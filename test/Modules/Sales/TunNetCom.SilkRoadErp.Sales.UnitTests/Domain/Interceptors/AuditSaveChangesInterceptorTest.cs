using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites.Interceptors;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Interceptors;

public class AuditSaveChangesInterceptorTest
{
    private static SalesContext CreateContext(
        out AuditSaveChangesInterceptor interceptor,
        Mock<ICurrentUserProvider>? userProvider = null)
    {
        var services = new ServiceCollection();
        if (userProvider != null)
        {
            services.AddSingleton(userProvider.Object);
        }
        var serviceProvider = services.BuildServiceProvider();
        interceptor = new AuditSaveChangesInterceptor(serviceProvider);

        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"AuditInterceptor_{Guid.NewGuid()}")
            .AddInterceptors(interceptor)
            .Options;

        return new SalesContext(options);
    }

    private static Mock<ICurrentUserProvider> CreateUserProvider(int? userId = 7, string? username = "john")
    {
        var mock = new Mock<ICurrentUserProvider>();
        _ = mock.Setup(p => p.GetUserId()).Returns(userId);
        _ = mock.Setup(p => p.GetUsername()).Returns(username);
        _ = mock.Setup(p => p.IsAuthenticated()).Returns(true);
        return mock;
    }

    private static Client CreateClient(string nom = "Alpha")
    {
        return Client.CreateClient(
            nom: nom,
            tel: "123",
            adresse: "Tunis",
            matricule: "M1",
            code: "C1",
            codeCat: "CAT1",
            etbSec: "ES1",
            mail: "alpha@test.com");
    }

    [Fact]
    public void SavingChanges_WhenEntityAdded_ShouldCreateCreatedAuditLog()
    {
        using var context = CreateContext(out _, CreateUserProvider());
        _ = context.Client.Add(CreateClient());
        _ = context.SaveChanges();

        var log = context.AuditLog.Should().ContainSingle().Subject;
        log.EntityName.Should().Be("Client");
        log.Action.Should().Be(AuditAction.Created);
        log.UserId.Should().Be(7);
        log.Username.Should().Be("john");
        log.NewValues.Should().NotBeNull();
        log.OldValues.Should().BeNull();
        log.ChangedProperties.Should().BeNull();
    }

    [Fact]
    public void SavingChanges_WhenEntityModified_ShouldCreateUpdatedAuditLogWithChanges()
    {
        using var context = CreateContext(out _, CreateUserProvider());
        var client = CreateClient("Alpha");
        _ = context.Client.Add(client);
        _ = context.SaveChanges();

        client.UpdateClient(
            nom: "Beta",
            tel: client.Tel,
            adresse: client.Adresse,
            matricule: client.Matricule,
            code: client.Code,
            codeCat: client.CodeCat,
            etbSec: client.EtbSec,
            mail: client.Mail);
        _ = context.SaveChanges();

        var updated = context.AuditLog.Single(l => l.Action == AuditAction.Updated);
        updated.EntityName.Should().Be("Client");
        updated.UserId.Should().Be(7);
        updated.OldValues.Should().NotBeNull();
        updated.NewValues.Should().NotBeNull();
        updated.ChangedProperties.Should().NotBeNull();
        updated.ChangedProperties.Should().Contain("Nom");
    }

    [Fact]
    public void SavingChanges_WhenEntityDeleted_ShouldCreateDeletedAuditLog()
    {
        using var context = CreateContext(out _, CreateUserProvider());
        var client = CreateClient();
        _ = context.Client.Add(client);
        _ = context.SaveChanges();

        _ = context.Client.Remove(client);
        _ = context.SaveChanges();

        var deleted = context.AuditLog.Single(l => l.Action == AuditAction.Deleted);
        deleted.EntityName.Should().Be("Client");
        deleted.OldValues.Should().NotBeNull();
        deleted.NewValues.Should().BeNull();
    }

    [Fact]
    public void SavingChanges_WhenNoUserProvider_ShouldUseSystemUsername()
    {
        using var context = CreateContext(out _, userProvider: null);
        _ = context.Client.Add(CreateClient());
        _ = context.SaveChanges();

        var log = context.AuditLog.Should().ContainSingle().Subject;
        log.Username.Should().Be("System");
        log.UserId.Should().BeNull();
    }

    [Fact]
    public void SavingChanges_WhenUsernameNull_ShouldFallbackToSystem()
    {
        using var context = CreateContext(out _, CreateUserProvider(username: null));
        _ = context.Client.Add(CreateClient());
        _ = context.SaveChanges();

        var log = context.AuditLog.Should().ContainSingle().Subject;
        log.Username.Should().Be("System");
    }

    [Fact]
    public void SavingChangesAsync_WhenEntityAdded_ShouldCreateAuditLog()
    {
        using var context = CreateContext(out _, CreateUserProvider());
        _ = context.Client.Add(CreateClient());
        _ = context.SaveChangesAsync().GetAwaiter().GetResult();

        var log = context.AuditLog.Should().ContainSingle().Subject;
        log.Action.Should().Be(AuditAction.Created);
        log.Username.Should().Be("john");
    }

    [Fact]
    public void SavingChanges_WhenEntityHasNoChanges_ShouldNotCreateAuditLog()
    {
        using var context = CreateContext(out _, CreateUserProvider());
        _ = context.Client.Add(CreateClient());
        _ = context.SaveChanges();

        // No new tracked changes, no new audit entries
        _ = context.SaveChanges();

        context.AuditLog.Should().ContainSingle();
    }
}
