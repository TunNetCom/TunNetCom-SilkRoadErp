# SilkRoadErp Docker Quick Reference

## 🚀 Quick Commands

### Build Images

```bash
# Build API image only
docker build --target api -t silkroaderp-api:latest .

# Build WebApp image only
docker build --target webapp -t silkroaderp-webapp:latest .

# Build both images
docker build --target api -t silkroaderp-api:latest .
docker build --target webapp -t silkroaderp-webapp:latest .
```

### Run with Docker Compose

```bash
# Start all services (API, WebApp, SQL Server, Seq)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: deletes data)
docker-compose down -v
```

### Run Individual Containers

```bash
# Run API container (requires SQL Server)
docker run -d \
  --name silkroaderp-api \
  -p 5000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Server=sqlserver;Database=SlikRoadErpDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;" \
  silkroaderp-api:latest

# Run WebApp container (requires API)
docker run -d \
  --name silkroaderp-webapp \
  -p 5001:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ApiSettings__BaseUrl=http://api:8080 \
  silkroaderp-webapp:latest
```

## 🔍 Useful Commands

### Inspect Containers

```bash
# List running containers
docker ps

# View container logs
docker logs silkroaderp-api
docker logs silkroaderp-webapp

# Access container shell
docker exec -it silkroaderp-api bash
docker exec -it silkroaderp-webapp bash

# Inspect container
docker inspect silkroaderp-api
```

### Database Operations

```bash
# Connect to SQL Server
docker exec -it silkroaderp-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C

# Backup database
docker exec -it silkroaderp-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C \
  -Q "BACKUP DATABASE [SlikRoadErpDB] TO DISK = '/var/opt/mssql/backup/SlikRoadErpDB.bak'"

# Copy backup to host
docker cp silkroaderp-sqlserver:/var/opt/mssql/backup/SlikRoadErpDB.bak ./backup/
```

### Cleanup

```bash
# Remove stopped containers
docker container prune

# Remove unused images
docker image prune -a

# Remove unused volumes
docker volume prune

# Complete cleanup (WARNING: removes everything)
docker system prune -a --volumes
```

## 🌐 Access URLs

After starting the services with `docker-compose up -d`:

- **Web Application**: http://localhost:5001
- **API**: http://localhost:5000
- **API Documentation (Swagger)**: http://localhost:5000/swagger
- **Seq Logs**: http://localhost:5341

## 🔧 Environment Variables

### API Service

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | Production |
| `ASPNETCORE_URLS` | URLs to listen on | http://+:8080 |
| `ConnectionStrings__DefaultConnection` | Database connection string | See docker-compose.yml |
| `Seq__ServerUrl` | Seq logging server URL | http://seq:80 |
| `JwtSettings__SecretKey` | JWT secret key | **CHANGE IN PRODUCTION** |
| `JwtSettings__Issuer` | JWT issuer | SilkRoadErp |
| `JwtSettings__Audience` | JWT audience | SilkRoadErp |
| `JwtSettings__AccessTokenExpirationMinutes` | Access token expiration | 30 |
| `JwtSettings__RefreshTokenExpirationDays` | Refresh token expiration | 7 |

### WebApp Service

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | Production |
| `ASPNETCORE_URLS` | URLs to listen on | http://+:8080 |
| `ApiSettings__BaseUrl` | API base URL | http://api:8080 |

## 🔒 Production Checklist

Before deploying to production:

- [ ] Change SQL Server SA password
- [ ] Change JWT secret key to a strong, random value (min 32 characters)
- [ ] Configure HTTPS/TLS certificates
- [ ] Set up proper backup strategy
- [ ] Configure resource limits in docker-compose.yml
- [ ] Use Docker secrets or environment-specific .env files
- [ ] Enable container scanning for vulnerabilities
- [ ] Set up monitoring and alerting
- [ ] Configure firewall rules
- [ ] Review and update CORS policies

## 📊 Monitoring

### Health Checks

```bash
# Check API health
curl http://localhost:5000/health

# Check WebApp health
curl http://localhost:5001/health

# Check container health status
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
```

### View Logs in Seq

1. Open http://localhost:5341 in your browser
2. View real-time logs from API and WebApp
3. Filter and search logs by level, timestamp, or custom properties

## 🐛 Troubleshooting

### Container won't start

```bash
# Check container logs
docker logs silkroaderp-api

# Check if port is already in use
netstat -ano | findstr :5000  # Windows
lsof -i :5000                 # Linux/Mac

# Restart container
docker restart silkroaderp-api
```

### Database connection issues

```bash
# Verify SQL Server is healthy
docker ps | grep sqlserver

# Check SQL Server logs
docker logs silkroaderp-sqlserver

# Test connection
docker exec -it silkroaderp-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -Q "SELECT @@VERSION"
```

### Build failures

```bash
# Clean build cache
docker builder prune

# Rebuild without cache
docker build --no-cache --target api -t silkroaderp-api:latest .

# Check .dockerignore file
cat .dockerignore
```

## 📚 Additional Resources

- [Official Docker Documentation](https://docs.docker.com/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [SQL Server Docker Images](https://hub.docker.com/_/microsoft-mssql-server)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
