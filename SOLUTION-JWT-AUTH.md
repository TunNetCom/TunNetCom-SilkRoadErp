# Solution au problème d'authentification JWT

## Problème identifié

L'utilisateur n'était pas authentifié dans `PermissionAuthorizationHandler`, causant des erreurs 401 Unauthorized lors de la création de documents.

### Causes racines

1. **AuthService est scoped** : Une nouvelle instance est créée pour chaque requête HTTP, donc le token stocké dans `AuthService.AccessToken` après le login n'était pas disponible dans les nouvelles instances.

2. **AuthHttpClientHandler ne chargeait pas systématiquement depuis localStorage** : Le handler tentait d'abord d'utiliser le token en mémoire, qui n'était souvent pas disponible.

3. **Manque de logs de diagnostic** : Difficile de savoir exactement où le token se perdait dans le flux.

## Solution implémentée

### 1. Amélioration de `AuthHttpClientHandler`

**Fichier** : `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/AuthHttpClientHandler.cs`

- ✅ Charge **toujours** le token depuis localStorage en premier (source de vérité)
- ✅ Fallback vers `AuthService` si localStorage échoue
- ✅ Gestion robuste des erreurs (prerendering, timeout, circuit disconnect)
- ✅ Logs détaillés pour diagnostiquer les problèmes :
  - Confirmation du chargement du token depuis localStorage
  - Affichage des premiers caractères du token
  - Vérification finale de l'Authorization header avant envoi
  - Log du status code de réponse

**Pourquoi ça marche** : Même si `AuthService` est réinstancié, localStorage persiste entre les requêtes et contient le token après le login.

### 2. Amélioration du stockage du token dans `AuthService`

**Fichier** : `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/AuthService.cs`

- ✅ Validation que le token n'est pas null avant stockage
- ✅ Stockage dans localStorage **avant** de définir `AccessToken` en mémoire
- ✅ Gestion d'erreur si le stockage échoue
- ✅ Logs pour tracer le processus de login

### 3. Vérification des permissions Manager

**Fichier** : `src/TunNetCom.SilkRoadErp.Sales.Api/Infrastructure/DataSeeder/DatabaseSeeder.cs`

- ✅ Logs pour confirmer les permissions assignées au rôle Manager
- ✅ Vérification que les permissions `CanCreate*` sont bien créées et assignées

### 4. Middleware de diagnostic d'authentification (NOUVEAU)

**Fichier** : `src/TunNetCom.SilkRoadErp.Sales.Api/Infrastructure/Middleware/AuthenticationDebugMiddleware.cs`

- ✅ Nouveau middleware qui s'exécute **après** `UseAuthentication()`
- ✅ Logs de l'état d'authentification pour chaque requête :
  - Présence du header Authorization
  - État `IsAuthenticated`
  - Nom d'utilisateur
  - Nombre de claims
  - Détails de tous les claims (mode debug)
- ✅ Alerte si le header est présent mais l'utilisateur n'est pas authentifié

**Position dans le pipeline** :
```csharp
app.UseAuthentication();
app.UseMiddleware<AuthenticationDebugMiddleware>(); // <-- ICI
app.UseAuthorization();
```

### 5. Amélioration des logs JWT dans `Program.cs`

**Fichier** : `src/TunNetCom.SilkRoadErp.Sales.Api/Program.cs`

- ✅ Log si `context.Principal` est null dans `OnTokenValidated`
- ✅ Définit explicitement `HttpContext.User = context.Principal` pour garantir la propagation

## Comment tester

1. **Redémarrer les deux applications** (API et WebApp)

2. **Se connecter avec un compte Manager** (ex: "Nieze" / mot de passe défini dans le seeder)

3. **Observer les logs côté WebApp** :
   ```
   AuthHttpClientHandler: ✓ Bearer token ADDED to request POST https://localhost:7139/invoices. Token length: 500, First 20 chars: eyJhbGciOiJIUzI1NiI...
   AuthHttpClientHandler: About to send request POST https://localhost:7139/invoices. Authorization header: Bearer eyJhbGciOiJIUzI1NiI...
   ```

4. **Observer les logs côté API** :
   ```
   OnMessageReceived - Request path: /invoices, Method: POST
   OnMessageReceived - Authorization header: Bearer eyJhbGciOiJIUzI1NiI...
   JWT Token extracted (length: 500)
   JWT Token validated successfully for user: Nieze, IsAuthenticated: True, Claims count: 5
   AuthDebug - Request: POST /invoices, HasAuthHeader: True, IsAuthenticated: True, UserName: Nieze, Claims: 5
   PermissionAuthorizationHandler: Checking permission 'CanCreateInvoice'
   PermissionAuthorizationHandler: User authenticated: True
   PermissionAuthorizationHandler: User 2 has permission 'CanCreateInvoice': True
   ```

5. **Créer un document** (facture, bon de livraison, devis, commande)

6. **Vérifier que la requête aboutit** (code 201 Created au lieu de 401 Unauthorized)

## Diagnostics en cas de problème persistant

Si l'utilisateur n'est toujours pas authentifié, vérifier dans les logs :

### Côté WebApp
- ✅ `AuthHttpClientHandler: ✓ Bearer token ADDED` → Token chargé et ajouté
- ❌ `AuthHttpClientHandler: ✗ NO TOKEN available` → Token non trouvé dans localStorage
  - **Solution** : Vérifier que le login fonctionne et stocke bien le token

### Côté API
- ✅ `OnMessageReceived - Authorization header: Bearer eyJ...` → Header présent
- ❌ `OnMessageReceived - Authorization header: MISSING` → Header non reçu
  - **Solution** : Vérifier le CORS, le proxy, ou la configuration du client HTTP

- ✅ `JWT Token validated successfully` → Token valide
- ❌ `JWT Authentication failed` → Token invalide ou expiré
  - **Solution** : Vérifier la clé JWT, l'issuer/audience, ou la date d'expiration

- ✅ `AuthDebug - IsAuthenticated: True` → Authentification réussie
- ❌ `AuthDebug - IsAuthenticated: False` → Authentification échouée malgré header présent
  - **Solution** : Vérifier la configuration JWT (clé, issuer, audience)

- ✅ `PermissionAuthorizationHandler: User authenticated: True` → Prêt pour vérification de permission
- ❌ `PermissionAuthorizationHandler: User authenticated: False` → Pas d'utilisateur dans le contexte
  - **Solution** : Vérifier que `HttpContext.User` est bien défini dans `OnTokenValidated`

## Fichiers modifiés

1. ✅ `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/AuthHttpClientHandler.cs`
2. ✅ `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/AuthService.cs`
3. ✅ `src/TunNetCom.SilkRoadErp.Sales.Api/Infrastructure/DataSeeder/DatabaseSeeder.cs`
4. ✅ `src/TunNetCom.SilkRoadErp.Sales.Api/Program.cs`
5. ✅ `src/TunNetCom.SilkRoadErp.Sales.Api/Infrastructure/Middleware/AuthenticationDebugMiddleware.cs` (NOUVEAU)

## Résumé

La solution garantit que :
1. Le token JWT est toujours chargé depuis localStorage (source de vérité persistante)
2. Des logs détaillés permettent de diagnostiquer tout problème de propagation du token
3. Un middleware de debug montre l'état d'authentification après le middleware JWT
4. Le token est correctement propagé du client à l'API avec des logs à chaque étape

Le flux complet est maintenant :
```
Login → localStorage → AuthHttpClientHandler → Authorization Header → JWT Middleware → HttpContext.User → PermissionAuthorizationHandler
```

Chaque étape est maintenant loggée pour faciliter le diagnostic.

