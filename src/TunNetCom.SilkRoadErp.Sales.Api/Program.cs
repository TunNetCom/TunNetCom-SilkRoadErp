
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();

builder.Services.AddDbContext<SalesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var assembly = typeof(Program).Assembly;

// MediatR
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblies(assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the exception handler
builder.Services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

// Use the exception handler
app.ConfigureExceptionHandler(); 

app.UseHttpsRedirection();

app.MapCarter();

app.Run();
