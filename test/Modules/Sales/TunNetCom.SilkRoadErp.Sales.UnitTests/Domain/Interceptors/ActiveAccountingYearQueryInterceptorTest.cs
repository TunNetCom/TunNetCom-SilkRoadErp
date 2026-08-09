using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites.Interceptors;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Interceptors;

public class ActiveAccountingYearQueryInterceptorTest
{
    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"ActiveYearInterceptor_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private static CommandEventData CreateEventData(SalesContext context, DbCommand command)
    {
        return new CommandEventData(
            null!,
            null!,
            null,
            command,
            null,
            context,
            DbCommandMethod.ExecuteReader,
            Guid.NewGuid(),
            Guid.NewGuid(),
            true,
            false,
            DateTimeOffset.Now,
            CommandSource.LinqQuery);
    }

    private static SqlCommand CreateCommand(string text)
    {
        return new SqlCommand(text);
    }

    private static ActiveAccountingYearQueryInterceptor CreateInterceptor(IServiceProvider? serviceProvider = null)
    {
        return new ActiveAccountingYearQueryInterceptor(serviceProvider ?? new ServiceCollection().BuildServiceProvider());
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenSelectOnAccountingYearTable_ShouldAddFilter()
    {
        SalesContext.SetActiveAccountingYearId(2024);
        try
        {
            using var context = CreateContext();
            var command = CreateCommand("SELECT [f].[Num] FROM [Facture] AS [f]");
            var interceptor = CreateInterceptor();
            var eventData = CreateEventData(context, command);

            await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

            command.CommandText.Should().Contain("[f].[AccountingYearId] = 2024");
            command.CommandText.Should().Contain("WHERE");
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenSelectHasWhere_ShouldAppendAndFilter()
    {
        SalesContext.SetActiveAccountingYearId(2024);
        try
        {
            using var context = CreateContext();
            var command = CreateCommand("SELECT [f].[Num] FROM [Facture] AS [f] WHERE [f].[Num] > 5");
            var interceptor = CreateInterceptor();
            var eventData = CreateEventData(context, command);

            await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

            command.CommandText.Should().Contain("[f].[AccountingYearId] = 2024");
            command.CommandText.Should().Contain("WHERE [f].[Num] > 5 AND [f].[AccountingYearId] = 2024");
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenSelectHasOrderBy_ShouldInsertFilterBeforeOrderBy()
    {
        SalesContext.SetActiveAccountingYearId(2024);
        try
        {
            using var context = CreateContext();
            var command = CreateCommand("SELECT [f].[Num] FROM [Facture] AS [f] ORDER BY [f].[Num]");
            var interceptor = CreateInterceptor();
            var eventData = CreateEventData(context, command);

            await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

            command.CommandText.Should().Contain("[f].[AccountingYearId] = 2024");
            command.CommandText.Should().NotContain("ORDER BY [f].[AccountingYearId]");
            command.CommandText.Should().Match("*AccountingYearId] = 2024*ORDER BY [f].[Num]*");
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenNoActiveYear_ShouldNotModifyCommand()
    {
        SalesContext.SetActiveAccountingYearId(null);
        using var context = CreateContext();
        var originalText = "SELECT [f].[Num] FROM [Facture] AS [f]";
        var command = CreateCommand(originalText);
        var interceptor = CreateInterceptor();
        var eventData = CreateEventData(context, command);

        await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

        command.CommandText.Should().Be(originalText);
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenNonSelectCommand_ShouldNotModify()
    {
        SalesContext.SetActiveAccountingYearId(2024);
        try
        {
            using var context = CreateContext();
            var originalText = "INSERT INTO [Facture] ([Num]) VALUES (1)";
            var command = CreateCommand(originalText);
            var interceptor = CreateInterceptor();
            var eventData = CreateEventData(context, command);

            await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

            command.CommandText.Should().Be(originalText);
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenNonAccountingYearTable_ShouldNotModify()
    {
        SalesContext.SetActiveAccountingYearId(2024);
        try
        {
            using var context = CreateContext();
            var originalText = "SELECT [c].[Id] FROM [Client] AS [c]";
            var command = CreateCommand(originalText);
            var interceptor = CreateInterceptor();
            var eventData = CreateEventData(context, command);

            await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

            command.CommandText.Should().Be(originalText);
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenExistingAccountingYearFilter_ShouldNotModify()
    {
        SalesContext.SetActiveAccountingYearId(2024);
        try
        {
            using var context = CreateContext();
            var originalText = "SELECT [f].[Num] FROM [Facture] AS [f] WHERE [f].[AccountingYearId] = 2023";
            var command = CreateCommand(originalText);
            var interceptor = CreateInterceptor();
            var eventData = CreateEventData(context, command);

            await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

            command.CommandText.Should().Be(originalText);
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenJoinQuery_ShouldNotModify()
    {
        SalesContext.SetActiveAccountingYearId(2024);
        try
        {
            using var context = CreateContext();
            var originalText = "SELECT [f].[Num] FROM [Facture] AS [f] INNER JOIN [BonDeLivraison] AS [b] ON [f].[Num] = [b].[NumFacture]";
            var command = CreateCommand(originalText);
            var interceptor = CreateInterceptor();
            var eventData = CreateEventData(context, command);

            await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

            command.CommandText.Should().Be(originalText);
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenExistsSubquery_ShouldNotModify()
    {
        SalesContext.SetActiveAccountingYearId(2024);
        try
        {
            using var context = CreateContext();
            var originalText = "SELECT [f].[Num] FROM [Facture] AS [f] WHERE EXISTS (SELECT 1 FROM [Client] AS [c] WHERE [c].[Id] = [f].[IdClient])";
            var command = CreateCommand(originalText);
            var interceptor = CreateInterceptor();
            var eventData = CreateEventData(context, command);

            await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

            command.CommandText.Should().Be(originalText);
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenContextNotSalesContext_ShouldNotModify()
    {
        SalesContext.SetActiveAccountingYearId(2024);
        try
        {
            var options = new DbContextOptionsBuilder<DbContext>()
                .UseInMemoryDatabase($"Other_{Guid.NewGuid()}")
                .Options;
            using var otherContext = new DbContext(options);
            var originalText = "SELECT [f].[Num] FROM [Facture] AS [f]";
            var command = CreateCommand(originalText);
            var interceptor = CreateInterceptor();

            var eventData = new CommandEventData(
                null!,
                null!,
                null,
                command,
                null,
                otherContext,
                DbCommandMethod.ExecuteReader,
                Guid.NewGuid(),
                Guid.NewGuid(),
                true,
                false,
                DateTimeOffset.Now,
                CommandSource.LinqQuery);

            await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

            command.CommandText.Should().Be(originalText);
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenServiceProvidesActiveYear_ShouldSetAndModify()
    {
        SalesContext.SetActiveAccountingYearId(null);
        try
        {
            var activeYearServiceMock = new Mock<IActiveAccountingYearService>();
            _ = activeYearServiceMock.Setup(s => s.GetActiveAccountingYearId()).Returns(2024);
            var services = new ServiceCollection();
            services.AddSingleton(activeYearServiceMock.Object);
            var serviceProvider = services.BuildServiceProvider();

            using var context = CreateContext();
            var command = CreateCommand("SELECT [f].[Num] FROM [Facture] AS [f]");
            var interceptor = CreateInterceptor(serviceProvider);
            var eventData = CreateEventData(context, command);

            await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

            command.CommandText.Should().Contain("[f].[AccountingYearId] = 2024");
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }

    [Fact]
    public async Task ReaderExecutingAsync_WhenServiceNull_ShouldNotThrow()
    {
        SalesContext.SetActiveAccountingYearId(null);
        using var context = CreateContext();
        var originalText = "SELECT [f].[Num] FROM [Facture] AS [f]";
        var command = CreateCommand(originalText);
        var interceptor = CreateInterceptor(); // empty service provider => GetService returns null
        var eventData = CreateEventData(context, command);

        var act = async () => await interceptor.ReaderExecutingAsync(command, eventData, default, CancellationToken.None);

        await act.Should().NotThrowAsync();
        command.CommandText.Should().Be(originalText);
    }

    [Fact]
    public void ReaderExecuting_WhenSelectOnAccountingYearTable_ShouldAddFilter()
    {
        SalesContext.SetActiveAccountingYearId(2024);
        try
        {
            using var context = CreateContext();
            var command = CreateCommand("SELECT [f].[Num] FROM [Facture] AS [f]");
            var interceptor = CreateInterceptor();
            var eventData = CreateEventData(context, command);

            interceptor.ReaderExecuting(command, eventData, default);

            command.CommandText.Should().Contain("[f].[AccountingYearId] = 2024");
        }
        finally
        {
            SalesContext.SetActiveAccountingYearId(null);
        }
    }
}
