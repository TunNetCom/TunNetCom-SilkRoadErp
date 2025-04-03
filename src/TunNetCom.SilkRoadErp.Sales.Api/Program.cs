using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices;

var builder = WebApplication.CreateBuilder(args);


var seqServerUrl = builder.Configuration["Seq:ServerUrl"];
// Configure Serilog
Serilog.Log.Logger = new LoggerConfiguration()
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

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblies(assembly));

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the exception handler
builder.Services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseSerilogRequestLogging();

// Use the exception handler
app.ConfigureExceptionHandler(); 

app.UseHttpsRedirection();

app.MapCarter();

try
{
    Serilog.Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Serilog.Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Serilog.Log.CloseAndFlush();
}
