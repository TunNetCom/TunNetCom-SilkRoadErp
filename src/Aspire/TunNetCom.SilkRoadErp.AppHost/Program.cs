using Aspire.Hosting.Lifecycle;
using CommunityToolkit.Aspire.Hosting.Dapr;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

// Fixed SQL Server sa password so it stays the same across restarts; connection strings and health checks always match.
// Override via Parameters__SqlPassword in env or user secrets if needed. Must meet SQL Server policy (8+ chars, upper, lower, digit, symbol).
var sqlPassword = builder.AddParameter("SqlPassword", "SilkRoad_SqlDev123!", secret: true);
var sql = builder.AddSqlServer("sql", password: sqlPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var salesDb = sql.AddDatabase("salesdb");
var adminDb = sql.AddDatabase("admindb");

var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder.AddRabbitMQ("rabbitmq");

// Using the IValueProvider overload gives the component a dynamically-resolved connection string (actual host
// port), but the toolkit's lifecycle hook turns it into a WaitUntilHealthy(rabbitmq-password) annotation on the
// component (it only sees the password parameter, not the rabbitmq container nested in the endpoint expression).
// In Aspire 13.3.4 a plain custom resource (component) that enters Waiting never resumes, deadlocking startup.
// DaprDependencyWaitWorkaround removes that wait and instead has the dapr CLI executables wait on the same
// resources their parent app waits for (rabbitmq), so the sidecars only start once rabbitmq is healthy (daprd
// exits fatally if the component cannot be initialized).
var pubsub = builder.AddDaprComponent("pubsub", "pubsub.rabbitmq")
    .WithMetadata("connectionString", rabbitmq.Resource.ConnectionStringExpression);

var loki = builder.AddContainer("loki", "grafana/loki", "latest")
    .WithHttpEndpoint(port: 3100, targetPort: 3100, name: "http");

builder.AddContainer("grafana", "grafana/grafana", "latest")
    .WithHttpEndpoint(port: 3000, targetPort: 3000, name: "http")
    .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin");

var salesApi = builder.AddProject<Projects.TunNetCom_SilkRoadErp_Sales_Api>("sales-api")
    .WithExternalHttpEndpoints()
    .WithReference(salesDb)
    .WaitFor(salesDb)
    .WithReference(redis)
    .WithEnvironment("ConnectionStrings__DefaultConnection", salesDb)
    .WithEnvironment("Loki__ServerUrl", loki.GetEndpoint("http"))
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithDaprSidecar(sidecar => sidecar
        .WithOptions(new DaprSidecarOptions
        {
            PlacementHostAddress = "",
            SchedulerHostAddress = ""
        })
        .WithReference(pubsub));

var adminApi = builder.AddProject<Projects.TunNetCom_SilkRoadErp_Administration_Api>("admin-api")
    .WithExternalHttpEndpoints()
    .WithReference(adminDb)
    .WaitFor(adminDb)
    .WithEnvironment("ConnectionStrings__AdminConnection", adminDb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithDaprSidecar(sidecar => sidecar
        .WithOptions(new DaprSidecarOptions
        {
            PlacementHostAddress = "",
            SchedulerHostAddress = ""
        })
        .WithReference(pubsub));

builder.AddProject<Projects.TunNetCom_SilkRoadErp_Sales_WebApp>("sales-webapp")
    .WithExternalHttpEndpoints()
    .WithReference(salesApi)
    .WaitFor(salesApi)
    .WithEnvironment("ApiSettings__BaseUrl", salesApi.GetEndpoint("https"));

builder.AddProject<Projects.TunNetCom_SilkRoadErp_Administration_WebApp>("admin-webapp")
    .WithExternalHttpEndpoints()
    .WithReference(adminApi)
    .WaitFor(adminApi)
    .WithEnvironment("AdminApi__BaseUrl", adminApi.GetEndpoint("https"));

builder.AddProject<Projects.TunNetCom_SilkRoadErp_TenantSetup_WebApp>("tenant-webapp")
    .WithExternalHttpEndpoints()
    .WithReference(adminApi)
    .WaitFor(adminApi)
    .WithEnvironment("AdminApi__BaseUrl", adminApi.GetEndpoint("https"));

// Must be registered after every WithDaprSidecar call so it runs after the toolkit's lifecycle hook.
builder.Services.TryAddEventingSubscriber<DaprDependencyWaitWorkaround>();

builder.Build().Run();
