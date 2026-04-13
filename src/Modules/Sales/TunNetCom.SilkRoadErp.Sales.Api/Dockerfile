# ==================================================================
# Stage 1: Build & Publish
# ==================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution & NuGet config
COPY *.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy source
COPY src/ ./src/

# Restore and publish API only
RUN dotnet restore src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj
RUN dotnet publish src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ==================================================================
# Stage 2: Runtime
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]
