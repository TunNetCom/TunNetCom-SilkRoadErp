using System.Threading.RateLimiting;
using System.Reflection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var seqServerUrl = builder.Configuration["Seq:ServerUrl"];
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(seqServerUrl)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCarter();

builder.Services.AddDbContext<SalesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    SalesContext dbContext = scope.ServiceProvider.GetRequiredService<SalesContext>();
    _ = dbContext.Database.EnsureCreated();
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

app.UseStaticFiles();
app.UseHttpsRedirection();

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