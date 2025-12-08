# ==============================================================================
# Multi-stage Dockerfile for SilkRoadErp Application + Playwright Support
# ==============================================================================

# ==============================================================================
# Stage 1: Base Runtime Image
# ==============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

# Install Chromium dependencies required by Playwright
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    curl \
    ca-certificates \
    libnss3 \
    libnspr4 \
    libatk1.0-0 \
    libatk-bridge2.0-0 \
    libcups2 \
    libdrm2 \
    libxkbcommon0 \
    libxcomposite1 \
    libxdamage1 \
    libxfixes3 \
    libxrandr2 \
    libgbm1 \
    libpango-1.0-0 \
    libcairo2 \
    libasound2 \
    && rm -rf /var/lib/apt/lists/*

# Create a non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser


# ==============================================================================
# Stage 2: Build Environment
# ==============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and props
COPY ["TunNetCom.SilkRoadErp.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project files
COPY ["src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj", "src/TunNetCom.SilkRoadErp.Sales.Api/"]
COPY ["src/TunNetCom.SilkRoadErp.Sales.Domain/TunNetCom.SilkRoadErp.Sales.Domain.csproj", "src/TunNetCom.SilkRoadErp.Sales.Domain/"]
COPY ["src/TunNetCom.SilkRoadErp.Sales.Contracts/TunNetCom.SilkRoadErp.Sales.Contracts.csproj", "src/TunNetCom.SilkRoadErp.Sales.Contracts/"]
COPY ["src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj", "src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/"]
COPY ["src/WebApps/TunNetCom.SilkRoadErp.Sales.HttpClients/TunNetCom.SilkRoadErp.Sales.HttpClients.csproj", "src/WebApps/TunNetCom.SilkRoadErp.Sales.HttpClients/"]

# Restore dependencies
RUN dotnet restore "src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj"
RUN dotnet restore "src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj"

# Copy source
COPY ["src/", "src/"]

# Build API
WORKDIR "/src/src/TunNetCom.SilkRoadErp.Sales.Api"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build/api

# Build WebApp
WORKDIR "/src/src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build/webapp


# ==============================================================================
# Stage 3: Publish API
# ==============================================================================
FROM build AS publish-api
WORKDIR "/src/src/TunNetCom.SilkRoadErp.Sales.Api"
RUN dotnet publish -c Release -o /app/publish/api --no-restore /p:UseAppHost=false


# ==============================================================================
# Stage 4: Publish WebApp
# ==============================================================================
FROM build AS publish-webapp
WORKDIR "/src/src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp"
RUN dotnet publish -c Release -o /app/publish/webapp --no-restore /p:UseAppHost=false


# ==============================================================================
# Stage 5: Playwright Browser Installation
# ==============================================================================
FROM build AS playwright-install

# Install Playwright CLI
RUN dotnet tool install --global Microsoft.Playwright.CLI

# Download Chromium + dependencies inside the layer
RUN playwright install --with-deps

# ==============================================================================
# Stage 6: Final API Image
# ==============================================================================
FROM base AS api
WORKDIR /app
COPY --from=publish-api /app/publish/api .

# Copy Playwright installed browsers
COPY --from=playwright-install /root/.cache/ms-playwright /home/appuser/.cache/ms-playwright

RUN chown -R appuser:appuser /home/appuser/.cache
RUN chown -R appuser:appuser /app
USER appuser

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]


# ==============================================================================
# Stage 7: Final WebApp Image
# ==============================================================================
FROM base AS webapp
WORKDIR /app
COPY --from=publish-webapp /app/publish/webapp .

# Copy Playwright installed browsers
COPY --from=playwright-install /root/.cache/ms-playwright /home/appuser/.cache/ms-playwright

RUN chown -R appuser:appuser /home/appuser/.cache
RUN chown -R appuser:appuser /app
USER appuser

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]
