# ==================================================================
# Stage 1: Build Stage (with SDK)
# ==================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution and NuGet configuration
COPY *.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy all projects
COPY src/ ./src/

# Restore dependencies for API and WebApp
RUN dotnet restore src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    && dotnet restore src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj

# Build API and WebApp
RUN dotnet build src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj -c Release -o /app/api \
    && dotnet build src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj -c Release -o /app/webapp

# Publish API
RUN dotnet publish src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    -c Release -o /app/api/publish

# ------------------------
# Install Playwright CLI & Chromium
# ------------------------
# Install Playwright CLI globally
RUN dotnet tool install --global Microsoft.Playwright.CLI

# Add .NET tools to PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Switch to API project folder
WORKDIR /src/src/TunNetCom.SilkRoadErp.Sales.Api

# Add Playwright NuGet package and build before installing browsers
RUN dotnet add package Microsoft.Playwright \
    && dotnet build -c Release \
    && playwright install chromium

# ==================================================================
# Stage 2: Runtime Stage for API
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api

WORKDIR /app

# Copy published API from build stage
COPY --from=build /app/api/publish ./

# Copy Playwright tools
COPY --from=build /root/.dotnet /root/.dotnet
ENV PATH="$PATH:/root/.dotnet/tools"

# Expose API port
EXPOSE 5000
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# Entry point
ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]

# ==================================================================
# Stage 3: Runtime Stage for WebApp
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS webapp

WORKDIR /app

# Copy built WebApp
COPY --from=build /app/webapp ./webapp

# Expose WebApp port
EXPOSE 80

# Optional: no ENTRYPOINT if served by API
