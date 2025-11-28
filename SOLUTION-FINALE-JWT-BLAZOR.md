# Solution FINALE au problème JWT + Blazor Server

## 🎯 Problème identifié

**Circuit IDs différents** : Les requêtes HTTP utilisaient des circuit IDs aléatoires différents, donc le token stocké lors du login n'était jamais retrouvé.

```
Login → TokenStore.SetToken(circuit "abc123", token)
HTTP Request → TokenStore.GetToken(circuit "xyz789") → NULL ❌
```

## ✅ Solution implémentée : Session-based Circuit ID

Au lieu d'utiliser un GUID aléatoire par scope, nous utilisons maintenant l'**ID de session HTTP** comme identifiant de circuit, garantissant la stabilité entre les requêtes.

### Fichiers créés/modifiés

1. **`TokenStore.cs`** (NOUVEAU) - Stockage Singleton des tokens par circuit
2. **`CircuitIdService.cs`** (MODIFIÉ) - Utilise `HttpContext.Session.Id` au lieu de `Guid.NewGuid()`
3. **`AuthService.cs`** (MODIFIÉ) - Utilise `TokenStore` + `CircuitIdService`
4. **`AuthHttpClientHandler.cs`** (MODIFIÉ) - Simplifié pour utiliser `AuthService.AccessToken`
5. **`Program.cs`** (MODIFIÉ) - Ajout de Session + `HttpContextAccessor`

### Architecture de la solution

```
┌─────────────────────────────────────────────────────────────┐
│                      User Login                              │
│  LoginPage → AuthService.LoginAsync()                       │
│           → TokenStore.SetToken(SessionId, JWT)             │
└─────────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                  HTTP Request (any page)                     │
│  Component → HttpClient → AuthHttpClientHandler             │
│           → AuthService.AccessToken (getter)                │
│           → TokenStore.GetToken(SessionId)                  │
│           → JWT Token Retrieved ✓                           │
│           → Authorization: Bearer {token}                   │
└─────────────────────────────────────────────────────────────┘
```

### Composants clés

#### 1. **TokenStore** (Singleton)
- Stocke les tokens JWT par Session ID
- Thread-safe (`ConcurrentDictionary`)
- Persiste pendant toute la durée de vie de l'application

#### 2. **CircuitIdService** (Scoped)
- Fournit un ID stable basé sur `HttpContext.Session.Id`
- Fallback sur `HttpContext.Connection.Id` si pas de session
- Même ID pour toutes les requêtes du même utilisateur

#### 3. **Session Middleware**
- Activé dans `Program.cs`
- Cookie de session : 2h d'inactivité
- HttpOnly + Essential pour sécurité

### Configuration ajoutée dans `Program.cs`

```csharp
// HttpContextAccessor pour accéder à la session
builder.Services.AddHttpContextAccessor();

// Session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Token storage
builder.Services.AddSingleton<ITokenStore, TokenStore>();
builder.Services.AddScoped<ICircuitIdService, CircuitIdService>();

// Middleware pipeline
app.UseSession(); // Avant MapRazorComponents
```

### Flux d'authentification complet

1. **Login** :
   ```
   User → LoginPage → AuthService.LoginAsync()
   → Token reçu de l'API
   → TokenStore.SetToken(SessionId, token)
   → localStorage.setItem (backup)
   ```

2. **Requête HTTP** :
   ```
   Component → HttpClient call
   → AuthHttpClientHandler.SendAsync()
   → CircuitIdService.GetCircuitId() → SessionId
   → AuthService.AccessToken → TokenStore.GetToken(SessionId)
   → Token trouvé ✓
   → request.Headers.Authorization = Bearer {token}
   ```

3. **Logout** :
   ```
   User → Logout
   → TokenStore.ClearToken(SessionId)
   → localStorage.removeItem()
   ```

### Avantages de cette solution

✅ **Fonctionne pendant le prerendering** - Pas besoin de JS interop  
✅ **Stable entre les requêtes** - Même Session ID pour tout le circuit  
✅ **Thread-safe** - `ConcurrentDictionary` dans TokenStore  
✅ **Pas de dépendance localStorage** - Fonctionne même si JS échoue  
✅ **Simple et maintenable** - Architecture claire  
✅ **Performant** - Lookup O(1) dans le dictionnaire

### Logs attendus après redémarrage

**Login** :
```
TokenStore: Token set for circuit a5f3c2b1. Token length: 500
Login successful for user Nieze. Token available in memory. Length: 500
```

**Requête HTTP** :
```
CircuitIdService: Using HttpContext-based ID: a5f3c2b1
TokenStore: Token retrieved for circuit a5f3c2b1. Length: 500
AuthHttpClientHandler: ✓ Token found in AuthService memory. Length: 500
AuthHttpClientHandler: ✓ Bearer token ADDED to request POST /quotations
```

**API** :
```
OnMessageReceived - Authorization header: Bearer eyJhbGciOiJIUzI1NiI...
JWT Token validated successfully for user: Nieze
AuthDebug: Request: POST /quotations, IsAuthenticated: True, UserName: Nieze
PermissionAuthorizationHandler: User authenticated: True
```

## 🚀 Prochaines étapes

1. **Arrêter les applications** (API + WebApp)
2. **Redémarrer les applications**
3. **Se connecter avec "Nieze"**
4. **Créer un document** (devis, facture, etc.)

**Le problème devrait être RÉSOLU !** ✅

## 📝 Notes techniques

- **Session Cookie Name** : `.AspNetCore.Session`
- **Session Idle Timeout** : 2 heures
- **TokenStore Capacity** : Illimité (garbage collected quand session expire)
- **Thread Safety** : Oui (`ConcurrentDictionary`)
- **Prerendering Compatibility** : ✅ Oui

## 🔒 Sécurité

- Les tokens sont stockés en mémoire serveur (plus sécurisé que localStorage)
- Session cookie HttpOnly (protection XSS)
- Session cookie Essential (fonctionne toujours)
- Les tokens sont automatiquement nettoyés à l'expiration de la session

## 🐛 Troubleshooting

Si le problème persiste :

1. **Vérifier que la session est active** :
   ```csharp
   _logger.LogInformation("Session ID: {SessionId}", HttpContext.Session.Id);
   ```

2. **Vérifier que le token est stocké** :
   ```csharp
   _logger.LogInformation("Token in store: {HasToken}", _tokenStore.GetToken(sessionId) != null);
   ```

3. **Vérifier que HttpContextAccessor fonctionne** :
   ```csharp
   var httpContext = _httpContextAccessor.HttpContext;
   _logger.LogInformation("HttpContext available: {Available}", httpContext != null);
   ```

## 📚 Références

- [ASP.NET Core Session](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state)
- [Blazor Server Authentication](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/)
- [HttpContextAccessor](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor)

