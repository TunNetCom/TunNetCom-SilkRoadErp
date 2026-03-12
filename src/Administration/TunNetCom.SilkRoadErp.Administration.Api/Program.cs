using Carter;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using TunNetCom.SilkRoadErp.Administration.Api.Hubs;
using TunNetCom.SilkRoadErp.Administration.Api.Infrastructure.Provisioning;
using TunNetCom.SilkRoadErp.Administration.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Prefer Aspire-injected URL so we don't bind to 5011 when running under AppHost (avoids "address already in use")
var aspireUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (!string.IsNullOrWhiteSpace(aspireUrls))
    builder.WebHost.UseUrls(aspireUrls);

builder.AddServiceDefaults();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCarter();
builder.Services.AddSignalR();

builder.Services.AddDbContext<AdminContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AdminConnection"),
        sql => sql.MigrationsAssembly("TunNetCom.SilkRoadErp.Administration.Domain")));

builder.Services.AddScoped<TenantProvisioningService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Initialize database with retries so SQL Server container can be ready under Aspire
const int maxAttempts = 10;
const int delayMs = 2000;
for (var attempt = 1; attempt <= maxAttempts; attempt++)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AdminContext>();
            await db.Database.EnsureCreatedAsync();

            if (!await db.BoundedContexts.AnyAsync())
            {
                var sales = new BoundedContext
                {
                    Key = "Sales",
                    Name = "Sales & Distribution",
                    Description = "Core sales, invoicing, delivery notes, credit notes, payments, and customer management",
                    Icon = "point_of_sale",
                    IsCore = true,
                    DisplayOrder = 1
                };
                sales.Features.Add(new Feature { Key = "Sales.Customers", Name = "Customers", Description = "Customer management", IsCore = true });
                sales.Features.Add(new Feature { Key = "Sales.Products", Name = "Products & Catalog", Description = "Product catalog, families, and sub-families", IsCore = true });
                sales.Features.Add(new Feature { Key = "Sales.Invoices", Name = "Invoicing", Description = "Invoice creation and management", IsCore = true });
                sales.Features.Add(new Feature { Key = "Sales.DeliveryNotes", Name = "Delivery Notes", Description = "Delivery note management", IsCore = true });
                sales.Features.Add(new Feature { Key = "Sales.Quotations", Name = "Price Quotations", Description = "Quotation and estimate management", IsCore = false });
                sales.Features.Add(new Feature { Key = "Sales.CreditNotes", Name = "Credit Notes", Description = "Customer credit notes (avoirs)", IsCore = false });
                sales.Features.Add(new Feature { Key = "Sales.CustomerPayments", Name = "Customer Payments", Description = "Customer payment tracking and reconciliation", IsCore = true });
                sales.Features.Add(new Feature { Key = "Sales.Orders", Name = "Orders", Description = "Sales order management", IsCore = false });
                sales.Features.Add(new Feature { Key = "Sales.Inventory", Name = "Inventory", Description = "Stock inventory management", IsCore = false });
                db.BoundedContexts.Add(sales);

                var purchasing = new BoundedContext
                {
                    Key = "Purchasing",
                    Name = "Purchasing & Procurement",
                    Description = "Supplier management, receipt notes, supplier invoices, supplier payments, and returns",
                    Icon = "shopping_cart",
                    IsCore = true,
                    DisplayOrder = 2
                };
                purchasing.Features.Add(new Feature { Key = "Purchasing.Providers", Name = "Suppliers", Description = "Supplier management", IsCore = true });
                purchasing.Features.Add(new Feature { Key = "Purchasing.ReceiptNotes", Name = "Receipt Notes", Description = "Goods receipt management", IsCore = true });
                purchasing.Features.Add(new Feature { Key = "Purchasing.ProviderInvoices", Name = "Supplier Invoices", Description = "Supplier invoice management", IsCore = true });
                purchasing.Features.Add(new Feature { Key = "Purchasing.ProviderPayments", Name = "Supplier Payments", Description = "Supplier payment tracking", IsCore = true });
                purchasing.Features.Add(new Feature { Key = "Purchasing.Returns", Name = "Supplier Returns", Description = "Return merchandise to suppliers", IsCore = false });
                purchasing.Features.Add(new Feature { Key = "Purchasing.CreditNotes", Name = "Supplier Credit Notes", Description = "Supplier credit notes (avoir fournisseur)", IsCore = false });
                db.BoundedContexts.Add(purchasing);

                var finance = new BoundedContext
                {
                    Key = "Finance",
                    Name = "Finance & Accounting",
                    Description = "Banking, expense invoices, third-party expense payments, withholding tax certificates, and accounting years",
                    Icon = "account_balance",
                    IsCore = false,
                    DisplayOrder = 3
                };
                finance.Features.Add(new Feature { Key = "Finance.Banking", Name = "Banking", Description = "Bank account and transaction management", IsCore = true });
                finance.Features.Add(new Feature { Key = "Finance.ExpenseInvoices", Name = "Expense Invoices", Description = "Operating expense invoice management", IsCore = false });
                finance.Features.Add(new Feature { Key = "Finance.ThirdPartyExpenses", Name = "Third-Party Expenses", Description = "Third-party expense tracking and payments", IsCore = false });
                finance.Features.Add(new Feature { Key = "Finance.TaxCertificates", Name = "Tax Certificates (TEJ)", Description = "Withholding tax certificate management", IsCore = false });
                finance.Features.Add(new Feature { Key = "Finance.AccountingYears", Name = "Accounting Years", Description = "Fiscal year management", IsCore = true });
                db.BoundedContexts.Add(finance);

                var platform = new BoundedContext
                {
                    Key = "Platform",
                    Name = "Platform & Administration",
                    Description = "User management, roles, audit logs, notifications, tags, and system configuration",
                    Icon = "settings",
                    IsCore = true,
                    DisplayOrder = 0
                };
                platform.Features.Add(new Feature { Key = "Platform.Users", Name = "User Management", Description = "User accounts and authentication", IsCore = true });
                platform.Features.Add(new Feature { Key = "Platform.AuditLogs", Name = "Audit Logs", Description = "Activity audit trail", IsCore = true });
                platform.Features.Add(new Feature { Key = "Platform.Notifications", Name = "Notifications", Description = "In-app notifications", IsCore = false });
                platform.Features.Add(new Feature { Key = "Platform.Tags", Name = "Tags & Labels", Description = "Document tagging system", IsCore = false });
                platform.Features.Add(new Feature { Key = "Platform.Dashboard", Name = "Dashboard", Description = "Overview dashboard and analytics", IsCore = true });
                db.BoundedContexts.Add(platform);

                await db.SaveChangesAsync();
            }
        }
        break;
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Admin DB init attempt {Attempt}/{Max} failed. Retrying in {Delay}ms.", attempt, maxAttempts, delayMs);
        if (attempt == maxAttempts)
        {
            logger.LogError(ex, "Admin DB init failed after {Max} attempts. API will run but admin features may fail until DB is available.", maxAttempts);
            break;
        }
        await Task.Delay(delayMs);
    }
}

app.UseCors();
app.MapOpenApi();
app.UseSerilogRequestLogging();

app.MapHub<ProvisioningHub>("/hubs/provisioning");
app.MapCarter();

app.MapDefaultEndpoints();

app.Run();
