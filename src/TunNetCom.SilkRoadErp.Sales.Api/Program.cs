
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCarter();

app.Run();
