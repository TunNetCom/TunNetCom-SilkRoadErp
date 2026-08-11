# ==================================================================
# Stage 1: Build & Publish
# ==================================================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY *.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY nuget.config ./
COPY src/ ./src/

ARG FEED_ACCESSTOKEN
ARG FEED_URL=https://pkgs.dev.azure.com/TunNetCom/Silk_Road_ERP/_packaging/SilkRoadFeed/nuget/v3/index.json
ARG FEED_USERNAME=docker

RUN curl -sSL https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | sh

RUN export VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"${FEED_URL}\", \"username\":\"${FEED_USERNAME}\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}" \
    && dotnet restore src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj

RUN dotnet publish \
    src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# IMPORTANT:
# Playwright .NET puts its Node executable inside the published app.
# The runtime container runs as UID 1000, so the binary must be executable.
RUN chmod 755 /app/publish/.playwright/node/linux-x64/node

# Install Playwright CLI and Chromium outside /root
RUN dotnet tool install Microsoft.Playwright.CLI --tool-path /opt/playwright-cli

ENV PATH="$PATH:/opt/playwright-cli"
ENV PLAYWRIGHT_BROWSERS_PATH=/opt/playwright

WORKDIR /src/src/Modules/Sales/TunNetCom.SilkRoadErp.Sales.WebApp

RUN playwright install chromium

RUN chmod -R a+rX /opt/playwright /opt/playwright-cli


# ==================================================================
# Stage 2: Runtime
# ==================================================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish ./

COPY --from=build /opt/playwright-cli /opt/playwright-cli
COPY --from=build /opt/playwright /opt/playwright

ENV PATH="$PATH:/opt/playwright-cli"
ENV PLAYWRIGHT_BROWSERS_PATH=/opt/playwright

RUN apt-get update && apt-get install -y \
    libglib2.0-0t64 \
    libnss3 \
    libnspr4 \
    libdbus-1-3 \
    libatk1.0-0t64 \
    libatk-bridge2.0-0t64 \
    libcups2t64 \
    libx11-6 \
    libxcomposite1 \
    libxdamage1 \
    libxext6 \
    libxfixes3 \
    libxrandr2 \
    libgbm1 \
    libxcb1 \
    libxkbcommon0 \
    libpango-1.0-0 \
    libcairo2 \
    libasound2t64 \
    libatspi2.0-0t64 \
    && rm -rf /var/lib/apt/lists/*

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]