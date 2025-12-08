# -------------------------
# BUILD STAGE
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore

# Build + Publish (no browser needed here)
RUN dotnet publish -c Release -o /app

# -------------------------
# RUNTIME STAGE
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

# Install Playwright + Chromium using the OFFICIAL script
RUN apt-get update && apt-get install -y wget gnupg ca-certificates \
    && wget https://playwright.azureedge.net/builds/driver/playwright-linux-amd64.tar.gz -O /tmp/playwright-driver.tar.gz \
    && mkdir -p /root/.cache/ms-playwright/ \
    && tar -xzf /tmp/playwright-driver.tar.gz -C /root/.cache/ms-playwright/ \
    && rm -rf /tmp/playwright-driver.tar.gz

# Install Playwright browsers (Chromium ONLY)
RUN apt-get update && apt-get install -y \
    libnss3 libatk1.0-0 libcups2 libdrm2 libxkbcommon0 libxcomposite1 \
    libxdamage1 libxfixes3 libxrandr2 libgbm1 libpango-1.0-0 libcairo2 libasound2 \
    && rm -rf /var/lib/apt/lists/*

# Copy published output
COPY --from=build /app ./

# Default
ENTRYPOINT ["dotnet", "TunNetCom.SilkRoadErp.Sales.WebApp.dll"]
