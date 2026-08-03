# ==================================================================
# Stage 1: Build & Publish
# ==================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution & NuGet config
COPY *.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY nuget.config ./

# Copy source
COPY src/ ./src/

# Azure Artifacts private-feed (SilkRoadFeed) auth for dotnet restore.
# FEED_ACCESSTOKEN is injected at build time by the pipeline
# (NuGetAuthenticate task); the artifacts credential provider resolves it.
# The token never reaches the final runtime image (separate stage).
ARG FEED_ACCESSTOKEN
ARG FEED_URL=https://pkgs.dev.azure.com/TunNetCom/Silk_Road_ERP/_packaging/SilkRoadFeed/nuget/v3/index.json
ARG FEED_USERNAME=docker
RUN curl -sSL https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | sh

# Restore and publish WebApp only
RUN export VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"${FEED_URL}\", \"username\":\"${FEED_USERNAME}\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}" \
    && dotnet restore src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj
RUN dotnet publish src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# Install Playwright CLI and Chromium
RUN dotnet tool install --global Microsoft.Playwright.CLI
ENV PATH="$PATH:/root/.dotnet/tools"
ENV PLAYWRIGHT_BROWSERS_PATH=/root/.playwright
WORKDIR /src/src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.WebApp
RUN playwright install chromium

# ==================================================================
# Stage 2: Runtime
# ==================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

# Copy published WebApp
COPY --from=build /app/publish ./

# Copy Playwright tools & browsers
COPY --from=build /root/.dotnet /root/.dotnet
COPY --from=build /root/.playwright /root/.playwright
ENV PATH="$PATH:/root/.dotnet/tools"
ENV PLAYWRIGHT_BROWSERS_PATH=/root/.playwright

# Install Linux dependencies required by Chromium
# List sourced from Playwright's own dependency detection for this base image
RUN apt-get update && apt-get install -y \
    libglib2.0-0t64 libnss3 libnspr4 libdbus-1-3 libatk1.0-0t64 \
    libatk-bridge2.0-0t64 libcups2t64 libx11-6 libxcomposite1 \
    libxdamage1 libxext6 libxfixes3 libxrandr2 libgbm1 libxcb1 \
    libxkbcommon0 libpango-1.0-0 libcairo2 libasound2t64 libatspi2.0-0t64 \
    && rm -rf /var/lib/apt/lists/*

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]
