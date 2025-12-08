# ==============================================================================
# Multi-stage Dockerfile for SilkRoadErp Application (API + WebApp)
# Includes Playwright + Chromium for PDF generation
# ==============================================================================

# ==============================================================================
# Stage 1: Build
# ==============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy API projects
COPY src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj src/TunNetCom.SilkRoadErp.Sales.Api/
COPY src/TunNetCom.SilkRoadErp.Sales.Domain/TunNetCom.SilkRoadErp.Sales.Domain.csproj src/TunNetCom.SilkRoadErp.Sales.Domain/
COPY src/TunNetCom.SilkRoadErp.Sales.Contracts/TunNetCom.SilkRoadErp.Sales.Contracts.csproj src/TunNetCom.SilkRoadErp.Sales.Contracts/

# Copy WebApp projects
COPY src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/
COPY src/WebApps/TunNetCom.SilkRoadErp.Sales.HttpClients/TunNetCom.SilkRoadErp.Sales.HttpClients.csproj src/WebApps/TunNetCom.SilkRoadErp.Sales.HttpClients/

# Restore dependencies
RUN dotnet restore

# Copy full source code
COPY src/ src/

# Publish API
RUN dotnet publish src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    -c Release -o /app/api /p:UseAppHost=false

# Publish WebApp
RUN dotnet publish src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj \
    -c Release -o /app/webapp /p:UseAppHost=false

# ==============================================================================
# Stage 2: Runtime API Image
# ==============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api
WORKDIR /app

# Install system dependencies required by Playwright / Chromium
RUN apt-get update && apt-get install -y \
    libnss3 libatk1.0-0 libcups2 libdrm2 libxkbcommon0 libxcomposite1 \
    libxdamage1 libxfixes3 libxrandr2 libgbm1 libpango-1.0-0 libcairo2 libasound2 \
    curl ca-certificates gnupg wget \
    && rm -rf /var/lib/apt/lists/*

# Copy published API
COPY --from=build /app/api ./

# Install Playwright CLI and Chromium
RUN dotnet tool install --global Microsoft.Playwright.CLI \
    && playwright install chromium

# Use non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app /root/.dotnet /root/.cache
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# API entrypoint
ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]

# ==============================================================================
# Stage 3: Runtime WebApp Image
# ==============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS webapp
WORKDIR /app

# Install system dependencies required by Playwright / Chromium
RUN apt-get update && apt-get install -y \
    libnss3 libatk1.0-0 libcups2 libdrm2 libxkbcommon0 libxcomposite1 \
    libxdamage1 libxfixes3 libxrandr2 libgbm1 libpango-1.0-0 libcairo2 libasound2 \
    curl ca-certificates gnupg wget \
    && rm -rf /var/lib/apt/lists/*

# Copy published WebApp
COPY --from=build /app/webapp ./

# Install Playwright CLI and Chromium
RUN dotnet tool install --global Microsoft.Playwright.CLI \
    && playwright install chromium

# Use non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app /root/.dotnet /root/.cache
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# WebApp entrypoint
ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]
