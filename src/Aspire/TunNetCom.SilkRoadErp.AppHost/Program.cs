var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var salesDb = sql.AddDatabase("salesdb");
var adminDb = sql.AddDatabase("admindb");

var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent);

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
    .WithEnvironment("Loki__ServerUrl", loki.GetEndpoint("http"));

var adminApi = builder.AddProject<Projects.TunNetCom_SilkRoadErp_Administration_Api>("admin-api")
    .WithExternalHttpEndpoints()
    .WithReference(adminDb)
    .WaitFor(adminDb)
    .WithEnvironment("ConnectionStrings__AdminConnection", adminDb);

builder.AddProject<Projects.TunNetCom_SilkRoadErp_Sales_WebApp>("sales-webapp")
    .WithExternalHttpEndpoints()
    .WithReference(salesApi)
    .WaitFor(salesApi)
    .WithEnvironment("ApiSettings__BaseUrl", salesApi.GetEndpoint("http"));

builder.AddProject<Projects.TunNetCom_SilkRoadErp_Administration_WebApp>("admin-webapp")
    .WithExternalHttpEndpoints()
    .WithReference(adminApi)
    .WaitFor(adminApi)
    .WithEnvironment("AdminApi__BaseUrl", adminApi.GetEndpoint("http"));

builder.AddProject<Projects.TunNetCom_SilkRoadErp_TenantSetup_WebApp>("tenant-webapp")
    .WithExternalHttpEndpoints()
    .WithReference(adminApi)
    .WaitFor(adminApi)
    .WithEnvironment("AdminApi__BaseUrl", adminApi.GetEndpoint("http"));

builder.Build().Run();
