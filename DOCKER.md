# Docker Deployment Guide for SilkRoadErp

This guide provides instructions for containerizing and deploying the SilkRoadErp application using Docker.

## 📋 Prerequisites

- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- Docker Compose v2.0 or higher
- At least 4GB of available RAM
- 10GB of free disk space

## 🏗️ Architecture

The application consists of the following services:

1. **API Service** - .NET 10 Web API (Port 5000)
2. **WebApp Service** - Blazor Server Application (Port 5001)
3. **SQL Server** - Database (Port 1433)
4. **Seq** - Centralized logging (Port 5341)

## � Dockerfile Structure

The project uses a **multi-stage Dockerfile** that builds both the API and WebApp services efficiently:

### Build Stages:
1. **base** - Runtime base image with .NET 10 ASP.NET runtime
2. **build** - SDK image for building the application
3. **publish-api** - Publishes the API project
4. **publish-webapp** - Publishes the WebApp project
5. **api** - Final API container image
6. **webapp** - Final WebApp container image

### Key Features:
- ✅ **Layer Caching**: Dependencies are restored separately for faster rebuilds
- ✅ **Security**: Runs as non-root user (`appuser`)
- ✅ **Health Checks**: Built-in health check endpoints
- ✅ **Optimized Size**: Multi-stage build reduces final image size
- ✅ **Production Ready**: Configured for production deployments

## �🚀 Quick Start

### Build and Run All Services

```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

### Access the Application

- **Web Application**: http://localhost:5001
- **API**: http://localhost:5000
- **API Documentation (Swagger)**: http://localhost:5000/swagger
- **Seq Logs**: http://localhost:5341

## 🔧 Configuration

### Environment Variables

#### API Service
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ConnectionStrings__DefaultConnection` - SQL Server connection string
- `Seq__ServerUrl` - Seq logging server URL
- `JwtSettings__SecretKey` - JWT secret key (MUST change in production)
- `JwtSettings__Issuer` - JWT issuer
- `JwtSettings__Audience` - JWT audience
- `JwtSettings__AccessTokenExpirationMinutes` - Access token expiration
- `JwtSettings__RefreshTokenExpirationDays` - Refresh token expiration

#### WebApp Service
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ApiSettings__BaseUrl` - API base URL

#### SQL Server
- `SA_PASSWORD` - SQL Server SA password (MUST change in production)
- `ACCEPT_EULA` - Accept SQL Server EULA (Y)
- `MSSQL_PID` - SQL Server edition (Developer/Express/Standard/Enterprise)

### Customizing Configuration

1. **Development Environment**:
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
   ```

2. **Production Environment**:
   Create a `docker-compose.prod.yml` file with production settings:
   ```yaml
   version: '3.8'
   services:
     api:
       environment:
         - JwtSettings__SecretKey=YOUR_PRODUCTION_SECRET_KEY_HERE
     sqlserver:
       environment:
         - SA_PASSWORD=YOUR_PRODUCTION_DB_PASSWORD_HERE
   ```
   
   Then run:
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
   ```

## 🔨 Building Individual Services

### Build API Only
```bash
docker build --target api -t silkroaderp-api:latest .
docker run -p 5000:8080 silkroaderp-api:latest
```

### Build WebApp Only
```bash
docker build --target webapp -t silkroaderp-webapp:latest .
docker run -p 5001:8080 silkroaderp-webapp:latest
```

## 📊 Database Management

### Initialize Database

The database will be created automatically on first run. To run migrations:

```bash
# Access the API container
docker exec -it silkroaderp-api bash

# Run migrations (if using EF Core migrations)
dotnet ef database update
```

### Backup Database

```bash
# Backup database
docker exec -it silkroaderp-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C \
  -Q "BACKUP DATABASE [SlikRoadErpDB] TO DISK = '/var/opt/mssql/backup/SlikRoadErpDB.bak'"

# Copy backup to host
docker cp silkroaderp-sqlserver:/var/opt/mssql/backup/SlikRoadErpDB.bak ./backup/
```

### Restore Database

```bash
# Copy backup to container
docker cp ./backup/SlikRoadErpDB.bak silkroaderp-sqlserver:/var/opt/mssql/backup/

# Restore database
docker exec -it silkroaderp-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C \
  -Q "RESTORE DATABASE [SlikRoadErpDB] FROM DISK = '/var/opt/mssql/backup/SlikRoadErpDB.bak' WITH REPLACE"
```

## 🔍 Monitoring and Debugging

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api
docker-compose logs -f webapp
docker-compose logs -f sqlserver
```

### Access Container Shell

```bash
# API container
docker exec -it silkroaderp-api bash

# WebApp container
docker exec -it silkroaderp-webapp bash

# SQL Server container
docker exec -it silkroaderp-sqlserver bash
```

### Health Checks

```bash
# Check service status
docker-compose ps

# Check API health
curl http://localhost:5000/health

# Check WebApp health
curl http://localhost:5001/health
```

## 🧹 Cleanup

### Remove Containers
```bash
docker-compose down
```

### Remove Containers and Volumes
```bash
docker-compose down -v
```

### Remove Images
```bash
docker-compose down --rmi all
```

### Complete Cleanup
```bash
docker-compose down -v --rmi all
docker system prune -a
```

## 🚢 Production Deployment

### Security Checklist

- [ ] Change default SQL Server SA password
- [ ] Change JWT secret key to a strong, random value
- [ ] Use environment-specific configuration files
- [ ] Enable HTTPS/TLS
- [ ] Configure firewall rules
- [ ] Set up proper backup strategy
- [ ] Configure resource limits
- [ ] Use secrets management (Docker Secrets, Azure Key Vault, etc.)
- [ ] Enable container scanning for vulnerabilities
- [ ] Set up monitoring and alerting

### Resource Limits

Add resource limits to `docker-compose.yml`:

```yaml
services:
  api:
    deploy:
      resources:
        limits:
          cpus: '1'
          memory: 1G
        reservations:
          cpus: '0.5'
          memory: 512M
```

### Using Docker Secrets

```bash
# Create secrets
echo "YourProductionPassword" | docker secret create db_password -
echo "YourJWTSecretKey" | docker secret create jwt_secret -

# Reference in docker-compose.yml
services:
  api:
    secrets:
      - jwt_secret
    environment:
      - JwtSettings__SecretKey_FILE=/run/secrets/jwt_secret
```

## 🌐 Cloud Deployment

### Azure Container Instances

```bash
# Login to Azure
az login

# Create resource group
az group create --name silkroaderp-rg --location eastus

# Create container instances
az container create \
  --resource-group silkroaderp-rg \
  --name silkroaderp-api \
  --image silkroaderp-api:latest \
  --dns-name-label silkroaderp-api \
  --ports 8080
```

### AWS ECS

```bash
# Push image to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.us-east-1.amazonaws.com
docker tag silkroaderp-api:latest <account-id>.dkr.ecr.us-east-1.amazonaws.com/silkroaderp-api:latest
docker push <account-id>.dkr.ecr.us-east-1.amazonaws.com/silkroaderp-api:latest
```

### Google Cloud Run

```bash
# Build and push to GCR
gcloud builds submit --tag gcr.io/PROJECT-ID/silkroaderp-api

# Deploy to Cloud Run
gcloud run deploy silkroaderp-api \
  --image gcr.io/PROJECT-ID/silkroaderp-api \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated
```

## 🐛 Troubleshooting

### Common Issues

1. **SQL Server won't start**
   - Ensure you have enough memory (minimum 2GB)
   - Check if port 1433 is already in use
   - Verify EULA acceptance

2. **API can't connect to database**
   - Wait for SQL Server health check to pass
   - Verify connection string
   - Check network connectivity

3. **WebApp can't connect to API**
   - Verify API is running: `docker-compose ps`
   - Check API URL configuration
   - Ensure services are on the same network

4. **Port conflicts**
   - Change port mappings in `docker-compose.yml`
   - Stop conflicting services

### Debug Mode

Run containers in debug mode:

```bash
docker-compose -f docker-compose.yml -f docker-compose.override.yml up
```

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [SQL Server Docker Images](https://hub.docker.com/_/microsoft-mssql-server)

## 📝 Notes

- Default SQL Server password: `YourStrong@Passw0rd` (CHANGE IN PRODUCTION!)
- Default JWT secret is for development only (CHANGE IN PRODUCTION!)
- Seq is optional and can be removed if not needed
- Data persists in Docker volumes even after container restart
- Use `.env` file for environment-specific variables

## 🤝 Support

For issues and questions, please contact: nieze.benmansour@outlook.com
