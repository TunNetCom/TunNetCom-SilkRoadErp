# ==================================================================
# Stage 1: Build & Publish Stage
# ==================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution & NuGet config
COPY *.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy only source projects (skip tests)
COPY src/ ./src/

# Restore only API and WebApp projects
RUN dotnet restore src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    && dotnet restore src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj

# Build & publish API
RUN dotnet publish src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    -c Release -o /app/api/publish

# Build & publish WebApp
RUN dotnet publish src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj \
    -c Release -o /app/webapp/publish

# ==================================================================
# Stage 2: Playwright Installation Stage
# ==================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS playwright-prep

# Install system dependencies required by Chromium FIRST
RUN apt-get update && apt-get install -y --no-install-recommends \
    libnss3 libatk1.0-0 libcups2 libxss1 libx11-xcb1 libxcomposite1 \
    libxdamage1 libxrandr2 libgbm1 libasound2 fonts-liberation libappindicator3-1 \
    libatk-bridge2.0-0 libgtk-3-0 libpangocairo-1.0-0 libxshmfence1 \
    libxkbcommon0 libgconf-2-4 \
 && rm -rf /var/lib/apt/lists/*

# Set Playwright browsers path
ENV PLAYWRIGHT_BROWSERS_PATH=/playwright-browsers
ENV PATH="$PATH:/root/.dotnet/tools"

# Install Playwright CLI globally
RUN dotnet tool install --global Microsoft.Playwright.CLI

# Install Playwright and download Chromium
WORKDIR /src
COPY src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj ./
RUN dotnet add package Microsoft.Playwright --version 1.48.0

# Download and install Chromium
RUN mkdir -p /playwright-browsers && \
    playwright install chromium --with-deps && \
    playwright install-deps chromium

# ==================================================================
# Stage 3: Runtime Stage for API
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api

WORKDIR /app

# Copy published API from build stage
COPY --from=build /app/api/publish ./

EXPOSE 5000
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]

# ==================================================================
# Stage 4: Runtime Stage for WebApp
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS webapp

WORKDIR /app

# Install system dependencies required by Chromium at runtime
RUN apt-get update && apt-get install -y --no-install-recommends \
    libnss3 libatk1.0-0 libcups2 libxss1 libx11-xcb1 libxcomposite1 \
    libxdamage1 libxrandr2 libgbm1 libasound2 fonts-liberation libappindicator3-1 \
    libatk-bridge2.0-0 libgtk-3-0 libpangocairo-1.0-0 libxshmfence1 \
    libxkbcommon0 libgconf-2-4 \
 && rm -rf /var/lib/apt/lists/*

# Copy published WebApp from build stage
COPY --from=build /app/webapp/publish ./

# Copy Playwright CLI and browsers from playwright-prep stage
COPY --from=playwright-prep /root/.dotnet /root/.dotnet
COPY --from=playwright-prep /playwright-browsers /playwright-browsers

# Set environment variables
ENV PATH="$PATH:/root/.dotnet/tools"
ENV PLAYWRIGHT_BROWSERS_PATH=/playwright-browsers
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]