# ==================================================================
# Stage 1: Build Stage (with SDK)
# ==================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution and NuGet configuration
COPY *.sln ./

# Copy Directory props for central package versions
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy all projects
COPY src/ ./src/

# Restore dependencies (all projects)
RUN dotnet restore src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    && dotnet restore src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj

# Build projects in Release mode
RUN dotnet build src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj -c Release -o /app/api \
    && dotnet build src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj -c Release -o /app/webapp

# Publish API
RUN dotnet publish src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    -c Release -o /app/api/publish

# Install Playwright CLI and Chromium in build stage
RUN dotnet tool install --global Microsoft.Playwright.CLI \
    && playwright install chromium

# ==================================================================
# Stage 2: Runtime Stage for API
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api

WORKDIR /app

# Copy published API from build stage
COPY --from=build /app/api/publish ./  

# Copy Playwright tools from build stage
COPY --from=build /root/.dotnet /root/.dotnet
ENV PATH="$PATH:/root/.dotnet/tools"

# Expose API port
EXPOSE 5000
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# Entry point
ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]

# ==================================================================
# Stage 3: Runtime Stage for WebApp (optional if separate)
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS webapp

WORKDIR /app

# Copy built WebApp
COPY --from=build /app/webapp ./webapp

# Expose WebApp port
EXPOSE 80

# No ENTRYPOINT needed if serving static files via WebApp's API or server
