# Authentication & Authorization System

This document describes the authentication (AuthN) and authorization (AuthZ) systems used in SilkRoad ERP.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              FRONTEND (Blazor Server)                        │
│  ┌─────────────┐    ┌─────────────────┐    ┌──────────────────────────────┐ │
│  │ AuthService │───▶│   TokenStore    │◀───│   CircuitIdService           │ │
│  │             │    │   (Singleton)   │    │   (Session-based stable ID)  │ │
│  └──────┬──────┘    └─────────────────┘    └──────────────────────────────┘ │
│         │                                                                    │
│         ▼                                                                    │
│  ┌──────────────────┐    ┌─────────────────────────┐                        │
│  │ localStorage     │    │ AuthHttpClientHandler   │                        │
│  │ (Refresh Token)  │    │ (Adds Bearer token)     │                        │
│  └──────────────────┘    └───────────┬─────────────┘                        │
└──────────────────────────────────────┼──────────────────────────────────────┘
                                       │
                                       ▼ HTTP + Bearer Token
┌──────────────────────────────────────────────────────────────────────────────┐
│                              BACKEND (ASP.NET Core API)                       │
│  ┌─────────────────┐    ┌─────────────────────────────────────────────────┐  │
│  │ JWT Middleware  │───▶│ PermissionAuthorizationHandler                  │  │
│  │ (Validates JWT) │    │ (Checks user permissions from database)         │  │
│  └─────────────────┘    └─────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────────────────┘
```

## Backend (API)

### JWT Configuration

Location: `src/TunNetCom.SilkRoadErp.Sales.Api/Program.cs`

```csharp
// JWT Settings from appsettings.json
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "SilkRoadErp",
            ValidAudience = "SilkRoadErp",
            ClockSkew = TimeSpan.FromMinutes(2) // Handles clock drift between servers
        };
    });
```

### Configuration (appsettings.json)

```json
{
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY_HERE",
    "Issuer": "SilkRoadErp",
    "Audience": "SilkRoadErp",
    "AccessTokenExpirationMinutes": 240,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Token Generation

Location: `src/TunNetCom.SilkRoadErp.Sales.Api/Infrastructure/Services/JwtTokenService.cs`

The `JwtTokenService` generates:
- **Access Token**: JWT containing user ID, username, email, roles, and permissions
- **Refresh Token**: Random 64-byte base64 string stored in database

### Permission-Based Authorization

Location: `src/TunNetCom.SilkRoadErp.Sales.Api/Infrastructure/Authorization/`

| File | Purpose |
|------|---------|
| `PermissionRequirement.cs` | Defines a permission requirement |
| `PermissionPolicyProvider.cs` | Creates authorization policies dynamically |
| `PermissionAuthorizationHandler.cs` | Validates user has required permission |
| `RequirePermissionAttribute.cs` | Attribute to protect endpoints |

Usage:
```csharp
[RequirePermission("CanCreateInvoice")]
public async Task<IResult> CreateInvoice(...) { }
```

### Auth Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/auth/login` | POST | Authenticate user, returns tokens |
| `/api/auth/refresh` | POST | Refresh access token using refresh token |
| `/api/auth/logout` | POST | Revoke refresh token |

---

## Frontend (Blazor Server WebApp)

### Key Services

#### 1. AuthService
Location: `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/AuthService.cs`

Responsibilities:
- Login/Logout
- Token storage (localStorage + in-memory TokenStore)
- Token refresh
- User info extraction from JWT

Key Methods:
```csharp
Task<bool> LoginAsync(string username, string password)
Task<bool> RefreshTokenAsync()
Task LogoutAsync()
Task LoadTokenFromStorageAsync()
Task<bool> EnsureValidTokenAsync() // Proactive token refresh
bool IsTokenValid() // Check if token expires within 5 minutes
```

#### 2. TokenStore
Location: `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/TokenStore.cs`

A singleton that stores JWT tokens keyed by circuit ID. This allows tokens to be shared across scoped services within the same user session.

#### 3. CircuitIdService
Location: `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/CircuitIdService.cs`

Generates a stable circuit ID per browser session:
1. Stores circuit ID in HttpContext.Session (persists across requests)
2. Falls back to Connection.Id if session unavailable
3. Generates GUID as last resort

This ensures all services within the same browser session share the same circuit ID.

#### 4. AuthHttpClientHandler
Location: `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/AuthHttpClientHandler.cs`

HTTP message handler that:
- Adds Bearer token to all outgoing requests
- Proactively refreshes token if expiring soon
- Automatically retries requests on 401 after token refresh

#### 5. PermissionService
Location: `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/PermissionService.cs`

Caches and checks user permissions from JWT claims.

```csharp
Task<bool> HasPermissionAsync(string permission)
Task<bool> HasAnyPermissionAsync(params string[] permissions)
Task<bool> HasAllPermissionsAsync(params string[] permissions)
```

#### 6. JwtAuthenticationStateProvider
Location: `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/JwtAuthenticationStateProvider.cs`

Blazor's AuthenticationStateProvider implementation that parses JWT claims to provide authentication state.

### UI Components

#### HasPermission Component
Location: `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Components/Authorization/HasPermission.razor`

```razor
<HasPermission Permission="CanCreateInvoice">
    <button>Create Invoice</button>
</HasPermission>

<HasPermission AnyPermissions='new[] { "CanViewUsers", "CanManageUsers" }'>
    <a href="/users">Users</a>
</HasPermission>
```

#### AuthorizeRouteView
Location: `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Components/AuthorizeRouteView.razor`

Custom route view that:
- Shows login page without auth check
- Loads token from localStorage on navigation
- Displays loading state while checking auth

---

## Token Flow

### Login Flow
```
1. User enters credentials
2. Frontend calls POST /api/auth/login
3. Backend validates credentials
4. Backend generates Access Token (JWT) + Refresh Token
5. Backend stores Refresh Token in database
6. Frontend stores Access Token in:
   - localStorage (persistence)
   - TokenStore (fast in-memory access)
7. Frontend stores Refresh Token in localStorage
```

### Request Flow
```
1. Component makes API call
2. AuthHttpClientHandler intercepts request
3. Checks if token is expiring soon → proactive refresh
4. Gets token from TokenStore (keyed by session circuit ID)
5. Falls back to localStorage if not in TokenStore
6. Adds Bearer token to request header
7. If 401 response → attempts token refresh → retries request
```

### Token Refresh Flow
```
1. AuthHttpClientHandler detects 401 or token expiring
2. Calls AuthService.RefreshTokenAsync()
3. AuthService gets refresh token from localStorage
4. Calls POST /api/auth/refresh
5. Backend validates refresh token, generates new tokens
6. Frontend updates localStorage and TokenStore
7. Original request retried with new token
```

---

## Troubleshooting

### Common Issues

#### 1. "No token found for circuit X"
**Cause**: Token stored under different circuit ID than the one being used.
**Solution**: The CircuitIdService now uses HttpContext.Session for stable IDs.

#### 2. "JavaScript interop calls cannot be issued"
**Cause**: JS interop called during prerendering.
**Solution**: All JS interop (localStorage access) is wrapped with try-catch and only called in `OnAfterRenderAsync`.

#### 3. Intermittent 401 Unauthorized
**Causes**:
- Token expired (check `AccessTokenExpirationMinutes`)
- Clock drift between servers (handled by 2-minute ClockSkew)
- Circuit ID mismatch (fixed by session-based IDs)

**Solution**: 
- Token is now proactively refreshed 5 minutes before expiry
- Automatic retry on 401 with fresh token

### Logging

Enable debug logging to trace auth issues:

```json
{
  "Logging": {
    "LogLevel": {
      "TunNetCom.SilkRoadErp.Sales.WebApp.Services": "Debug"
    }
  }
}
```

Key log messages:
- `CircuitIdService: Using session-stored circuit ID: {id}`
- `TokenStore: Token set for circuit {id}`
- `AuthHttpClientHandler: 401 UNAUTHORIZED - attempting token refresh`
- `RefreshTokenAsync: TOKEN REFRESH SUCCESSFUL`

---

## Security Considerations

1. **Secret Key**: Change `JwtSettings:SecretKey` in production
2. **HTTPS**: Always use HTTPS in production
3. **Token Expiration**: Access token = 4 hours, Refresh token = 7 days
4. **Refresh Token Rotation**: Old refresh token is revoked on each refresh
5. **ClockSkew**: 2-minute tolerance for clock drift between servers

