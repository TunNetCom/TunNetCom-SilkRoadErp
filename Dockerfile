# -------------------------
# BUILD STAGE
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore

# Build + Publish API
RUN dotnet publish "src/TunNetCom.SilkRoadErp.Sales.Api/TunNetCom.SilkRoadErp.Sales.Api.csproj" \
    -c Release -o /app/api /p:UseAppHost=false

# Build + Publish WebApp
RUN dotnet publish "src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/TunNetCom.SilkRoadErp.Sales.WebApp.csproj" \
    -c Release -o /app/webapp /p:UseAppHost=false

# -------------------------
# RUNTIME STAGE - API
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api
WORKDIR /app
COPY --from=build /app/api ./ 

# Playwright + Chromium dependencies
RUN apt-get update && apt-get install -y wget gnupg ca-certificates \
    && wget https://playwright.azureedge.net/builds/driver/playwright-linux-amd64.tar.gz -O /tmp/playwright-driver.tar.gz \
    && mkdir -p /root/.cache/ms-playwright/ \
    && tar -xzf /tmp/playwright-driver.tar.gz -C /root/.cache/ms-playwright/ \
    && rm -rf /tmp/playwright-driver.tar.gz

RUN apt-get update && apt-get install -y \
    libnss3 libatk1.0-0 libcups2 libdrm2 libxkbcommon0 libxcomposite1 \
    libxdamage1 libxfixes3 libxrandr2 libgbm1 libpango-1.0-0 libcairo2 libasound2 \
    && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.Api.dll"]

# -------------------------
# RUNTIME STAGE - WebApp
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS webapp
WORKDIR /app
COPY --from=build /app/webapp ./ 

# Playwright + Chromium dependencies
RUN apt-get update && apt-get install -y wget gnupg ca-certificates \
    && wget https://playwright.azureedge.net/builds/driver/playwright-linux-amd64.tar.gz -O /tmp/playwright-driver.tar.gz \
    && mkdir -p /root/.cache/ms-playwright/ \
    && tar -xzf /tmp/playwright-driver.tar.gz -C /root/.cache/ms-playwright/ \
    && rm -rf /tmp/playwright-driver.tar.gz

RUN apt-get update && apt-get install -y \
    libnss3 libatk1.0-0 libcups2 libdrm2 libxkbcommon0 libxcomposite1 \
    libxdamage1 libxfixes3 libxrandr2 libgbm1 libpango-1.0-0 libcairo2 libasound2 \
    && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]
