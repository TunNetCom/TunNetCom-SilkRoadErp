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

# Restore only API and WebApp projects (skip missing UnitTests)
RUN dotnet restore src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    && dotnet restore src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj

# Build & publish API
RUN dotnet publish src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj \
    -c Release -o /app/api/publish

# Build & publish WebApp
RUN dotnet publish src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj \
    -c Release -o /app/webapp/publish

# Install Playwright CLI globally
RUN dotnet tool install --global Microsoft.Playwright.CLI
ENV PATH="$PATH:/root/.dotnet/tools"

# Set Playwright browsers path explicitly BEFORE installation
ENV PLAYWRIGHT_BROWSERS_PATH=/root/.playwright

# Install Playwright for WebApp and download Chromium
WORKDIR /src/src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp
RUN dotnet add package Microsoft.Playwright \
    && dotnet build -c Release \
    && playwright install chromium

# ==================================================================
# Stage 2: Runtime Stage for API
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api

WORKDIR /app

# Copy published API
COPY --from=build /app/api/publish ./

# Copy Playwright tools if API needs it
COPY --from=build /root/.dotnet /root/.dotnet
ENV PATH="$PATH:/root/.dotnet/tools"

EXPOSE 5000
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]

# ==================================================================
# Stage 3: Runtime Stage for WebApp
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS webapp

WORKDIR /app

# Copy published WebApp
COPY --from=build /app/webapp/publish ./

# Copy Playwright tools & browsers (now guaranteed to exist)
COPY --from=build /root/.dotnet /root/.dotnet
COPY --from=build /root/.playwright /root/.playwright
ENV PATH="$PATH:/root/.dotnet/tools"
ENV PLAYWRIGHT_BROWSERS_PATH=/root/.playwright

# Install Linux dependencies required by Chromium
RUN apt-get update && apt-get install -y \
    libnss3 libatk1.0-0 libcups2 libxss1 libx11-xcb1 libxcomposite1 \
    libxdamage1 libxrandr2 libgbm1 libasound2t64 fonts-liberation libappindicator3-1 \
    libatk-bridge2.0-0 libgtk-3-0 libpangocairo-1.0-0 libxshmfence1 \
 && rm -rf /var/lib/apt/lists/*

# Set .NET environment variables
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]
