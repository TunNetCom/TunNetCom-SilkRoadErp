# 🐛 Fix : Audit Log affiche "System" au lieu du nom d'utilisateur

## 🔍 PROBLÈME IDENTIFIÉ

L'audit log enregistrait toujours "System" dans la colonne `Username` au lieu du vrai nom de l'utilisateur connecté.

### Cause Racine

Dans `AuditSaveChangesInterceptor.cs` (ligne 52) :
```csharp
var username = currentUserProvider?.GetUsername() ?? "System";
```

Le problème était que `GetUsername()` retournait `null` et on ne savait pas pourquoi (pas de logs).

---

## ✅ SOLUTION APPLIQUÉE

### 1. Ajout de Logs Détaillés dans `CurrentUserService`

**Fichier** : `src/TunNetCom.SilkRoadErp.Sales.Api/Infrastructure/Services/CurrentUserService.cs`

**Changements** :
- ✅ Ajouté un `ILogger<CurrentUserService>` injecté
- ✅ Logs dans `GetUserId()` :
  - Warning si `HttpContext` est null
  - Warning si `HttpContext.User` est null
  - Warning si le claim `NameIdentifier` n'existe pas
  - Liste de tous les claims disponibles pour debugging
  - Debug quand userId est trouvé
- ✅ Logs dans `GetUsername()` :
  - Warning si `HttpContext` est null
  - Warning si `HttpContext.User` est null
  - Warning si le claim `Name` n'existe pas
  - Liste de tous les claims disponibles pour debugging
  - **Debug quand username est trouvé**
- ✅ Logs dans `IsAuthenticated()` :
  - Debug du statut d'authentification

**Exemple de logs attendus** :
```
[DEBUG] GetUsername: Found username=Nieze
[DEBUG] GetUserId: Found userId=2
[DEBUG] IsAuthenticated: True
```

**Si ça ne marche pas** :
```
[WARNING] GetUsername: Name claim not found. Available claims: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier=2, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress=nieze@example.com, permission=CanCreateInvoice, permission=CanViewInvoices, ...
```

---

### 2. Ajout de Logs dans `AuditSaveChangesInterceptor`

**Fichier** : `src/TunNetCom.SilkRoadErp.Sales.Domain/Entites/Interceptors/AuditSaveChangesInterceptor.cs`

**Changements** :
- ✅ Ajouté `using Microsoft.Extensions.Logging;`
- ✅ Log Warning si `CurrentUserProvider` est null
- ✅ Log Debug avec `UserId`, `Username`, et `IsAuthenticated` avant chaque audit

**Exemple de logs attendus** :
```
[DEBUG] AuditLog: UserId=2, Username=Nieze, IsAuthenticated=True
```

**Si ça ne marche pas** :
```
[WARNING] AuditLog: CurrentUserProvider is NULL
```
OU
```
[DEBUG] AuditLog: UserId=2, Username=null, IsAuthenticated=True
```

---

## 🧪 TESTS À EFFECTUER

### 1. Redémarrer l'API

```bash
cd src/TunNetCom.SilkRoadErp.Sales.Api
dotnet run
```

**Important** : Arrêter toutes les instances de l'API en cours d'exécution avant de redémarrer.

---

### 2. Se Connecter avec un Utilisateur

1. Ouvrir le WebApp
2. Se connecter avec **Nieze** (Manager) ou **admin**
3. Créer/Modifier/Supprimer une entité (Facture, Produit, Client, etc.)

---

### 3. Vérifier les Logs dans la Console de l'API

**Logs attendus** (BONS SIGNES ✅) :

```
[DEBUG] GetUsername: Found username=Nieze
[DEBUG] GetUserId: Found userId=2
[DEBUG] IsAuthenticated: True
[DEBUG] AuditLog: UserId=2, Username=Nieze, IsAuthenticated=True
```

**Logs indiquant un problème** (⚠️) :

```
[WARNING] GetUsername: HttpContext is NULL
```
→ Le `IHttpContextAccessor` ne fonctionne pas correctement

```
[WARNING] GetUsername: HttpContext.User is NULL
```
→ L'utilisateur n'est pas authentifié ou le middleware d'authentification n'a pas été exécuté

```
[WARNING] GetUsername: Name claim not found. Available claims: ...
```
→ Le JWT ne contient pas le claim `ClaimTypes.Name` (voir la liste des claims disponibles)

```
[DEBUG] AuditLog: UserId=2, Username=null, IsAuthenticated=True
```
→ L'utilisateur est authentifié, a un ID, mais pas de claim `Name`

---

### 4. Vérifier la Base de Données

```sql
SELECT TOP 10 
    Id,
    EntityName,
    EntityId,
    Action,
    UserId,
    Username,  -- ⬅️ Cette colonne devrait contenir "Nieze" ou "admin", pas "System"
    Timestamp
FROM AuditLog
ORDER BY Timestamp DESC;
```

**Résultat attendu** :
| UserId | Username | Action | EntityName |
|--------|----------|--------|-----------|
| 2 | Nieze | Created | Invoice |
| 2 | Nieze | Updated | Product |
| 1 | admin | Deleted | Customer |

**Résultat MAUVAIS** (actuel) :
| UserId | Username | Action | EntityName |
|--------|----------|--------|-----------|
| NULL | System | Created | Invoice |
| NULL | System | Updated | Product |

---

## 🔍 DIAGNOSTIC

### Scénario 1 : `Username=null` mais `UserId` est présent

**Symptôme** :
```
[DEBUG] AuditLog: UserId=2, Username=null, IsAuthenticated=True
[WARNING] GetUsername: Name claim not found. Available claims: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier=2, ...
```

**Cause possible** :
Le JWT ne contient pas le claim `http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name`.

**Solution** :
Vérifier `JwtTokenService.cs` ligne 39 :
```csharp
new Claim(ClaimTypes.Name, user.Username),
```

Le problème pourrait être que `user.Username` est null au moment de la génération du token.

---

### Scénario 2 : `UserId=null` et `Username=null`

**Symptôme** :
```
[DEBUG] AuditLog: UserId=null, Username=null, IsAuthenticated=False
[WARNING] GetUsername: HttpContext.User is NULL
```

**Cause possible** :
L'utilisateur n'est PAS authentifié au moment où l'audit log est créé.

**Solution** :
Vérifier que l'endpoint API a bien `.RequireAuthorization()` ou `.RequireAuthorization("Permission:...")`.

---

### Scénario 3 : `CurrentUserProvider is NULL`

**Symptôme** :
```
[WARNING] AuditLog: CurrentUserProvider is NULL
```

**Cause possible** :
Le `ICurrentUserProvider` n'est pas enregistré dans le DI container.

**Solution** :
Vérifier dans `Program.cs` :
```csharp
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
// Ensure CurrentUserService implements both ICurrentUserService AND ICurrentUserProvider
```

**Et que** `CurrentUserService` implémente les deux interfaces (c'est déjà le cas ✅) :
```csharp
public class CurrentUserService : ICurrentUserService, ICurrentUserProvider
```

---

## 📝 PROCHAINES ÉTAPES

1. ✅ **Tester** avec l'utilisateur Nieze
2. ✅ **Vérifier les logs** dans la console de l'API
3. ✅ **Vérifier la BDD** (table `AuditLog`)
4. ⏳ **Si ça ne marche toujours pas** :
   - Copier les logs de l'API ici
   - Vérifier que l'endpoint appelé a bien `.RequireAuthorization()`
   - Vérifier que le JWT contient bien le claim `Name`

---

## ✅ RÉSUMÉ DES CHANGEMENTS

| Fichier | Changement | Status |
|---------|-----------|--------|
| `CurrentUserService.cs` | Ajout de logs détaillés | ✅ Compilé |
| `AuditSaveChangesInterceptor.cs` | Ajout de logs détaillés | ✅ Compilé |

**Compilation** : ✅ **SUCCÈS** (0 erreurs)

---

## 🎯 OBJECTIF

Après ces changements, les logs vont nous dire **exactement** pourquoi `GetUsername()` retourne `null`, ce qui nous permettra de corriger le problème de manière ciblée.

**Si les logs montrent que le claim `Name` existe mais n'est pas récupéré**, on saura qu'il y a un problème avec `ClaimTypes.Name`.

**Si les logs montrent que le claim `Name` n'existe pas**, on saura qu'il y a un problème avec la génération du JWT dans `JwtTokenService`.

**Si les logs montrent que `HttpContext` est null**, on saura qu'il y a un problème avec le `IHttpContextAccessor`.

---

**MAINTENANT : Teste et partage-moi les logs ! 📋**

