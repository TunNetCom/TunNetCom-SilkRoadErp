# ==============================================================================
# Multi-stage Dockerfile for SilkRoadErp Application
# This Dockerfile builds both the API and WebApp services
# ==============================================================================

# ==============================================================================
# Stage 1: Base Runtime Image
# ==============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install necessary dependencies and configure environment
# Include Chromium dependencies for Playwright
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    curl \
    ca-certificates \
    libnss3 \
    libatk-bridge2.0-0 \
    libdrm2 \
    libxkbcommon0 \
    libxcomposite1 \
    libxdamage1 \
    libxfixes3 \
    libxrandr2 \
    libgbm1 \
    libasound2 \
    libpango-1.0-0 \
    libcairo2 \
    libatk1.0-0 \
    libcups2 \
    fonts-liberation \
    libappindicator3-1 \
    xdg-utils && \
    rm -rf /var/lib/apt/lists/*

# Create a non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# ==============================================================================
# Stage 2: Build Environment
# ==============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files first (for better layer caching)
COPY ["TunNetCom.SilkRoadErp.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy all project files
COPY ["src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj", "src/TunNetCom.SilkRoadErp.Sales.Api/"]
COPY ["src/TunNetCom.SilkRoadErp.Sales.Domain/TunNetCom.SilkRoadErp.Sales.Domain.csproj", "src/TunNetCom.SilkRoadErp.Sales.Domain/"]
COPY ["src/TunNetCom.SilkRoadErp.Sales.Contracts/TunNetCom.SilkRoadErp.Sales.Contracts.csproj", "src/TunNetCom.SilkRoadErp.Sales.Contracts/"]
COPY ["src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj", "src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/"]
COPY ["src/WebApps/TunNetCom.SilkRoadErp.Sales.HttpClients/TunNetCom.SilkRoadErp.Sales.HttpClients.csproj", "src/WebApps/TunNetCom.SilkRoadErp.Sales.HttpClients/"]

# Restore dependencies (cached if project files haven't changed)
RUN dotnet restore "src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj"
RUN dotnet restore "src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj"

# Copy the rest of the source code
COPY ["src/", "src/"]

# Build the projects
WORKDIR "/src/src/TunNetCom.SilkRoadErp.Sales.Api"
RUN dotnet build "TunNetCom.SilkRoadErp.Sales.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build/api

WORKDIR "/src/src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp"
RUN dotnet build "TunNetCom.SilkRoadErp.Sales.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build/webapp

# ==============================================================================
# Stage 3: Publish API
# ==============================================================================
FROM build AS publish-api
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/src/TunNetCom.SilkRoadErp.Sales.Api"
RUN dotnet publish "TunNetCom.SilkRoadErp.Sales.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish/api \
    --no-restore \
    /p:UseAppHost=false

# ==============================================================================
# Stage 4: Publish WebApp
# ==============================================================================
FROM build AS publish-webapp
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp"
RUN dotnet publish "TunNetCom.SilkRoadErp.Sales.WebApp.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish/webapp \
    --no-restore \
    /p:UseAppHost=false

# Install Playwright browsers (Chromium only to reduce image size)
WORKDIR /app/publish/webapp
# Use dotnet tool to install Playwright CLI, then install Chromium browser
# Ensure the cache directory exists even if installation fails
RUN mkdir -p /root/.cache/ms-playwright && \
    dotnet tool install --global Microsoft.Playwright.CLI && \
    export PATH="$PATH:/root/.dotnet/tools" && \
    playwright install chromium || \
    (echo "Warning: Playwright installation may have failed, but continuing..." && true)

# ==============================================================================
# Stage 5: Final API Image
# ==============================================================================
FROM base AS api
WORKDIR /app
COPY --from=publish-api /app/publish/api .

# Change ownership to non-root user
RUN chown -R appuser:appuser /app
USER appuser

# Health check endpoint
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]

# ==============================================================================
# Stage 6: Final WebApp Image
# ==============================================================================
FROM base AS webapp
WORKDIR /app
COPY --from=publish-webapp /app/publish/webapp .

# Copy Playwright browsers from publish-webapp stage to appuser's cache directory
# The browsers were installed in /root/.cache/ms-playwright during publish-webapp stage
RUN mkdir -p /home/appuser/.cache/ms-playwright

# Copy Playwright cache from the publish stage
# The directory should exist (created in publish-webapp stage), but if it doesn't, 
# the service will install browsers at runtime
COPY --from=publish-webapp /root/.cache/ms-playwright /home/appuser/.cache/ms-playwright

# Change ownership to non-root user (including Playwright cache)
RUN chown -R appuser:appuser /app && \
    chown -R appuser:appuser /home/appuser/.cache 2>/dev/null || true

# Set environment variable for Playwright
ENV PLAYWRIGHT_BROWSERS_PATH=/home/appuser/.cache/ms-playwright

USER appuser

# Health check endpoint
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]
