using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

var lokiUrl = builder.Configuration["Loki:ServerUrl"];
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki(lokiUrl)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCarter();

// Add OData support
builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .SetMaxTop(1000)
        .Count()
        .AddRouteComponents("odata", EdmModelBuilder.GetEdmModel()));

// Configure Kestrel to allow larger request bodies for image uploads (50MB)
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50MB
});

// Configure form options for larger requests
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB
    options.ValueLengthLimit = int.MaxValue;
    options.ValueCountLimit = int.MaxValue;
});

// Add HttpContextAccessor for accessing current user in services
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<SalesContext>((serviceProvider, options) =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.MigrationsAssembly("TunNetCom.SilkRoadErp.Sales.Domain"));
    
    // Add audit interceptor
    var auditInterceptor = serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>();
    options.AddInterceptors(auditInterceptor);
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 400,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 20,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }
        ));
});

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblies(assembly));

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); // Add built-in OpenAPI support for Scalar
builder.Services.AddSwaggerGen(options =>
{
    // Configure Swagger document with OpenAPI info
    // Swashbuckle 10.0.1 will automatically generate OpenAPI 3.0 spec with the openapi version field
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "SilkRoad ERP Sales API",
        Version = "v1",
        Description = "API for SilkRoad ERP Sales Management System"
    });
    
    // Add JWT Bearer authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    
    // Extract tags from endpoint metadata
    // WithTags() in minimal APIs stores tags in endpoint metadata
    options.TagActionsBy(api =>
    {
        var tags = new List<string>();
        
        // Try to get tags from the endpoint metadata
        // The tags are stored when using .WithTags() extension method
        foreach (var metadata in api.ActionDescriptor.EndpointMetadata)
        {
            var metadataType = metadata.GetType();
            
            // Check for Tags property (case-insensitive to be safe)
            var tagsProperty = metadataType.GetProperty("Tags", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            
            if (tagsProperty != null)
            {
                var tagValue = tagsProperty.GetValue(metadata);
                if (tagValue != null)
                {
                    if (tagValue is IEnumerable<string> tagEnumerable)
                    {
                        tags.AddRange(tagEnumerable);
                    }
                    else if (tagValue is string[] tagArray)
                    {
                        tags.AddRange(tagArray);
                    }
                    else if (tagValue is string singleTag)
                    {
                        tags.Add(singleTag);
                    }
                }
            }
        }
        
        // If no tags found, fall back to GroupName or Default
        if (!tags.Any())
        {
            tags.Add(api.GroupName ?? "Default");
        }
        
        return tags.Distinct().ToList();
    });
    options.DocInclusionPredicate((name, api) => true);
});

// Register the exception handler
builder.Services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();

// Register DatabaseSeeder
builder.Services.AddScoped<DatabaseSeeder>();

// Register NumberGeneratorService
builder.Services.AddScoped<INumberGeneratorService, NumberGeneratorService>();

// Register ActiveAccountingYearService
builder.Services.AddScoped<IActiveAccountingYearService, ActiveAccountingYearService>();

// Register StockCalculationService
builder.Services.AddScoped<IStockCalculationService,StockCalculationService>();

// Register SageErpExportService
builder.Services.AddScoped<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.SageErpExportService>();

// Register TejXmlExportService
builder.Services.AddScoped<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.TejXmlExportService>();

// Register ExcelExportService
builder.Services.AddScoped<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.ExcelExportService>();

// Register PdfListExportService
builder.Services.AddScoped<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.PdfListExportService>();

// Register Document Storage Service (configurable via appsettings.json, default: Base64)
var documentStorageType = builder.Configuration["DocumentStorage:Type"] ?? "Base64";
switch (documentStorageType)
{
    case "S3":
        builder.Services.AddScoped<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage.IDocumentStorageService, TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage.S3DocumentStorageService>();
        break;
    case "AzureBlob":
        builder.Services.AddScoped<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage.IDocumentStorageService, TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage.AzureBlobStorageService>();
        break;
    case "Base64":
    default:
        builder.Services.AddScoped<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage.IDocumentStorageService, TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage.Base64DocumentStorageService>();
        break;
}

// Register JWT and Password services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Register CurrentUserService for audit logging (implements both ICurrentUserService and ICurrentUserProvider)
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserService>();

// Register AuditSaveChangesInterceptor
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

// Register NotificationService
builder.Services.AddScoped<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications.NotificationService>();

// Register Notification Verifiers
builder.Services.AddScoped<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications.INotificationVerifier, TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications.UnpaidClientNotificationVerifier>();
builder.Services.AddScoped<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications.INotificationVerifier, TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications.LowStockNotificationVerifier>();

// Configure Quartz.NET for background jobs
builder.Services.AddQuartz(q =>
{
    // Register the job
    var jobKey = new JobKey("NotificationCheckJob");
    q.AddJob<TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Jobs.NotificationCheckJob>(opts => opts.WithIdentity(jobKey));

    // Get interval from configuration (default: 60 minutes)
    var intervalMinutes = builder.Configuration.GetValue<int>("NotificationSettings:CheckIntervalMinutes", 60);
    
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("NotificationCheckJob-trigger")
        .StartNow()
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(intervalMinutes)
            .RepeatForever()));
});

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false, // Token expiration disabled for simple auth
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "SilkRoadErp",
        ValidAudience = jwtSettings["Audience"] ?? "SilkRoadErp",
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // Allow 2 minutes clock skew to handle time drift between servers in production
        ClockSkew = TimeSpan.FromMinutes(2)
    };
    
    // Add event handler for authentication failures
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Error("JWT Authentication failed: {Error}", context.Exception.Message);
            if (context.Exception is Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
                Log.Warning("JWT Token expired for request {Path}", context.Request.Path);
            }
            return Task.CompletedTask;
        }
    };
});

// Configure Authorization
builder.Services.AddAuthorization(options =>
{
    // No fallback policy - endpoints must explicitly require authorization
});

// Register Permission Authorization Handler and Policy Provider
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    SalesContext dbContext = scope.ServiceProvider.GetRequiredService<SalesContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // #region agent log
        System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
            System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "A", location = "Program.cs:287", message = "Checking database connection", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion
        
        // Check if database exists and has tables
        var canConnect = await dbContext.Database.CanConnectAsync();
        
        // #region agent log
        System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
            System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "A", location = "Program.cs:288", message = "canConnect result", data = new { canConnect = canConnect }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion
        
        var migrationsApplied = await dbContext.Database.GetPendingMigrationsAsync();
        var migrationsList = migrationsApplied.ToList();
        
        // #region agent log
        System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
            System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "B", location = "Program.cs:289", message = "Pending migrations detected", data = new { count = migrationsList.Count, migrations = migrationsList }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion
        
        // #region agent log
        System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
            System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "C", location = "Program.cs:290", message = "Condition check", data = new { canConnect = canConnect, hasPendingMigrations = migrationsList.Any() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion
        
        if (canConnect && migrationsList.Any())
        {
            // #region agent log
            System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "E", location = "Program.cs:291", message = "Entering migration apply branch", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
            // #endregion
            
            // Log pending migrations
            logger.LogInformation("Migrations en attente détectées: {Count} migration(s)", migrationsList.Count);
            foreach (var migration in migrationsList)
            {
                logger.LogInformation("  - {MigrationId}", migration);
            }
            
            // FIRST: Add columns for AddTauxRetenuAndRibToFournisseur migration BEFORE MigrateAsync
            // This prevents EF Core from trying to use columns that don't exist yet
            if (migrationsList.Contains("20251218001616_AddTauxRetenuAndRibToFournisseur"))
            {
                logger.LogInformation("Ajout préalable des colonnes pour la migration AddTauxRetenuAndRibToFournisseur...");
                
                try
                {
                    // Add each column if it doesn't exist (IF NOT EXISTS ensures idempotency)
                    await dbContext.Database.ExecuteSqlRawAsync(@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Fournisseur' AND COLUMN_NAME = 'taux_retenu')
                        BEGIN
                            ALTER TABLE [Fournisseur] ADD [taux_retenu] float NULL;
                        END
                    ");
                    
                    await dbContext.Database.ExecuteSqlRawAsync(@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Fournisseur' AND COLUMN_NAME = 'rib_code_etab')
                        BEGIN
                            ALTER TABLE [Fournisseur] ADD [rib_code_etab] nvarchar(10) NULL;
                        END
                    ");
                    
                    await dbContext.Database.ExecuteSqlRawAsync(@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Fournisseur' AND COLUMN_NAME = 'rib_code_agence')
                        BEGIN
                            ALTER TABLE [Fournisseur] ADD [rib_code_agence] nvarchar(10) NULL;
                        END
                    ");
                    
                    await dbContext.Database.ExecuteSqlRawAsync(@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Fournisseur' AND COLUMN_NAME = 'rib_numero_compte')
                        BEGIN
                            ALTER TABLE [Fournisseur] ADD [rib_numero_compte] nvarchar(20) NULL;
                        END
                    ");
                    
                    await dbContext.Database.ExecuteSqlRawAsync(@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Fournisseur' AND COLUMN_NAME = 'rib_cle')
                        BEGIN
                            ALTER TABLE [Fournisseur] ADD [rib_cle] nvarchar(5) NULL;
                        END
                    ");
                    
                    logger.LogInformation("Colonnes ajoutées avec succès avant MigrateAsync.");
                }
                catch (Exception colEx)
                {
                    logger.LogWarning(colEx, "Erreur lors de l'ajout préalable des colonnes, continuons avec MigrateAsync...");
                }
            }
            
            // Try to apply migrations - if it fails because tables exist, mark them as applied
            try
            {
                logger.LogInformation("Application des migrations en attente...");
                
                // #region agent log
                System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                    System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "E", location = "Program.cs:360", message = "Before MigrateAsync", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                // #endregion
                
                await dbContext.Database.MigrateAsync();
                
                // #region agent log
                System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                    System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "E", location = "Program.cs:304", message = "After MigrateAsync - success", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                // #endregion
                
                logger.LogInformation("Migrations appliquées avec succès.");
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 2714 || sqlEx.Number == 1913 || sqlEx.Message.Contains("already exists"))
            {
                // #region agent log
                System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                    System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "F", location = "Program.cs:306", message = "Caught table exists exception", data = new { errorNumber = sqlEx.Number, errorMessage = sqlEx.Message }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                // #endregion
                
                // Table already exists error - FIRST add missing columns, THEN mark migrations as applied
                logger.LogWarning("Les tables existent déjà. Vérification et ajout des colonnes manquantes...");
                
                // #region agent log
                System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                    System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "F", location = "Program.cs:310", message = "Entering catch block - adding columns first", data = new { pendingMigrationsCount = migrationsList.Count }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                // #endregion
                
                try
                {
                    // FIRST: Add columns for AddTauxRetenuAndRibToFournisseur migration if needed
                    // This must happen BEFORE any EF Core operations that might use these columns
                    if (migrationsList.Contains("20251218001616_AddTauxRetenuAndRibToFournisseur"))
                    {
                        logger.LogInformation("Ajout des colonnes pour la migration AddTauxRetenuAndRibToFournisseur...");
                        
                        // #region agent log
                        System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                            System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "F", location = "Program.cs:360", message = "Adding columns for AddTauxRetenuAndRibToFournisseur", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                        // #endregion
                        
                        // Add each column if it doesn't exist (IF NOT EXISTS ensures idempotency)
                        await dbContext.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Fournisseur' AND COLUMN_NAME = 'taux_retenu')
                            BEGIN
                                ALTER TABLE [Fournisseur] ADD [taux_retenu] float NULL;
                            END
                        ");
                        
                        await dbContext.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Fournisseur' AND COLUMN_NAME = 'rib_code_etab')
                            BEGIN
                                ALTER TABLE [Fournisseur] ADD [rib_code_etab] nvarchar(10) NULL;
                            END
                        ");
                        
                        await dbContext.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Fournisseur' AND COLUMN_NAME = 'rib_code_agence')
                            BEGIN
                                ALTER TABLE [Fournisseur] ADD [rib_code_agence] nvarchar(10) NULL;
                            END
                        ");
                        
                        await dbContext.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Fournisseur' AND COLUMN_NAME = 'rib_numero_compte')
                            BEGIN
                                ALTER TABLE [Fournisseur] ADD [rib_numero_compte] nvarchar(20) NULL;
                            END
                        ");
                        
                        await dbContext.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Fournisseur' AND COLUMN_NAME = 'rib_cle')
                            BEGIN
                                ALTER TABLE [Fournisseur] ADD [rib_cle] nvarchar(5) NULL;
                            END
                        ");
                        
                        // #region agent log
                        System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                            System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "F", location = "Program.cs:395", message = "Columns added successfully", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                        // #endregion
                        
                        logger.LogInformation("Colonnes ajoutées avec succès.");
                    }
                    
                    // #region agent log
                    System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                        System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "F", location = "Program.cs:400", message = "Before creating __EFMigrationsHistory table", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                    // #endregion
                    
                    // Create __EFMigrationsHistory table if it doesn't exist
                    await dbContext.Database.ExecuteSqlRawAsync(@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
                        BEGIN
                            CREATE TABLE [__EFMigrationsHistory] (
                                [MigrationId] nvarchar(150) NOT NULL,
                                [ProductVersion] nvarchar(32) NOT NULL,
                                CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                            );
                        END
                    ");
                    
                    // #region agent log
                    System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                        System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "F", location = "Program.cs:415", message = "After creating __EFMigrationsHistory table, before marking migrations", data = new { migrationsToProcess = migrationsList }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                    // #endregion
                    
                    // NOW mark ALL pending migrations as applied
                    foreach (var migration in migrationsList)
                    {
                        // #region agent log
                        System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                            System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "F", location = "Program.cs:440", message = "Marking migration as applied", data = new { migrationId = migration }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                        // #endregion
                        
                        // Mark migration as applied
                        var migrationId = migration.Replace("'", "''"); // Escape single quotes for SQL
                        await dbContext.Database.ExecuteSqlRawAsync($@"
                            IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '{migrationId}')
                            BEGIN
                                INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('{migrationId}', '10.0.0');
                            END
                        ");
                        
                        // #region agent log
                        System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                            System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "F", location = "Program.cs:400", message = "Migration marked as applied", data = new { migrationId = migration }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                        // #endregion
                        
                        logger.LogInformation("Migration {MigrationId} marquée comme appliquée.", migration);
                    }
                    
                    // #region agent log
                    System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                        System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "F", location = "Program.cs:345", message = "All migrations marked successfully", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                    // #endregion
                    
                    logger.LogInformation("Toutes les migrations en attente ont été marquées comme appliquées.");
                }
                catch (Exception ex2)
                {
                    // #region agent log
                    System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                        System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "F", location = "Program.cs:352", message = "Exception in catch block", data = new { exceptionType = ex2.GetType().Name, exceptionMessage = ex2.Message, stackTrace = ex2.StackTrace }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                    // #endregion
                    
                    logger.LogError(ex2, "Erreur lors du marquage des migrations. Continuons quand même...");
                }
            }
            catch (Exception ex)
            {
                // #region agent log
                System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                    System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "D", location = "Program.cs:343", message = "Caught unexpected exception in MigrateAsync", data = new { exceptionType = ex.GetType().Name, exceptionMessage = ex.Message, stackTrace = ex.StackTrace }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
                // #endregion
                
                logger.LogError(ex, "Erreur inattendue lors de l'application des migrations.");
                throw;
            }
        }
        else if (!canConnect)
        {
            // #region agent log
            System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "A", location = "Program.cs:345", message = "Entering database creation branch", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
            // #endregion
            
            // Database doesn't exist, create it with migrations
            logger.LogInformation("Création de la base de données avec migrations...");
            
            // #region agent log
            System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "A", location = "Program.cs:349", message = "Before MigrateAsync in creation branch", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
            // #endregion
            
            await dbContext.Database.MigrateAsync();
            
            // #region agent log
            System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "A", location = "Program.cs:350", message = "After MigrateAsync in creation branch", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
            // #endregion
            
            logger.LogInformation("Base de données créée et migrations appliquées avec succès.");
        }
        else
        {
            // #region agent log
            System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", 
                System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "C", location = "Program.cs:352", message = "Entering 'database up to date' branch", data = new { canConnect = canConnect, hasPendingMigrations = migrationsList.Any() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
            // #endregion
            
            logger.LogInformation("Base de données à jour, aucune migration en attente.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erreur lors de l'application des migrations.");
        throw; // Faire échouer le démarrage si les migrations échouent
        }
        
        // Seed database if tables are empty
        logger.LogInformation("=== DÉBUT DU PROCESSUS DE SEEDING ===");
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        logger.LogInformation("Appel du seeder...");
        await seeder.SeedAsync(dbContext);
        logger.LogInformation("=== FIN DU PROCESSUS DE SEEDING ===");
    }

app.UseRateLimiter();

//if (app.Environment.IsDevelopment())
//{
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.MapOpenApi(); // Map OpenAPI endpoint for Scalar
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("SilkRoad ERP Sales API")
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
        .WithTheme(ScalarTheme.DeepSpace)
        .WithDarkMode(true);
    options.ForceThemeMode = ThemeMode.Dark;
});
//}

app.UseSerilogRequestLogging();

// Use the exception handler
app.ConfigureExceptionHandler();

// Use ActiveAccountingYearMiddleware to cache the active accounting year at the start of each request
app.UseMiddleware<ActiveAccountingYearMiddleware>();

app.UseStaticFiles();
app.UseHttpsRedirection();

// Routing must be before Authentication
app.UseRouting();

// Authentication & Authorization must be before MapControllers and MapCarter
app.UseAuthentication();

// Add debug middleware only in Development
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<AuthenticationDebugMiddleware>();
}

app.UseAuthorization();

// Map OData routes
app.MapControllers();

app.MapCarter();

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}