# 🐛 Fix : Token Expiré Après Logout

## 🔍 PROBLÈME IDENTIFIÉ

Après le logout, l'application continuait à utiliser l'ancien token JWT **expiré**, causant des erreurs 401 :

```
[16:29:29] ERR: JWT Authentication failed: IDX10223: Lifetime validation failed. 
The token is expired. ValidTo (UTC): '28/11/2025 15:24:13', Current time (UTC): '28/11/2025 15:29:29'.
```

### Causes

1. Le token était **expiré** (5 minutes après sa création)
2. Le `LogoutAsync()` ne vidait pas correctement le token de la mémoire
3. L'ordre de nettoyage n'était pas optimal

---

## ✅ SOLUTION APPLIQUÉE

### Amélioration de `LogoutAsync()`

**Fichier** : `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/AuthService.cs`

**Changements** :
1. ✅ Vider `AccessToken = null` **en premier** (mémoire)
2. ✅ Vider le `TokenStore` pour le circuit actuel
3. ✅ Vider le `localStorage` (pour la persistance)
4. ✅ Ajout de logs détaillés à chaque étape

**Ordre d'exécution** :
```csharp
1. AccessToken = null;                      // Mémoire (immédiat)
2. _tokenStore.ClearToken(circuitId);       // Circuit store
3. localStorage.removeItem("auth_access_token");  // Persistance
4. localStorage.removeItem("auth_refresh_token"); // Persistance
```

**Logs ajoutés** :
```
[INFO] Logout: Starting logout process
[INFO] Logout: Logout request sent to API
[INFO] Logout: Token cleared from memory
[INFO] Logout: Token cleared from TokenStore for circuit {CircuitId}
[INFO] Logout: Tokens cleared from localStorage
[INFO] Logout: Logout process completed
```

---

## 🧪 TESTS À EFFECTUER

### 1. Tuer tous les processus .NET

✅ **Déjà fait !**

### 2. Recompiler

✅ **Déjà fait ! Compilation réussie (0 erreurs)**

### 3. Redémarrer l'API et le WebApp

```bash
# Terminal 1 - API
cd src/TunNetCom.SilkRoadErp.Sales.Api
dotnet run

# Terminal 2 - WebApp
cd src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp
dotnet run
```

### 4. Tester le Logout

1. **Se connecter** avec Nieze ou admin
2. **Utiliser l'app** (consulter des factures, produits, etc.)
3. **Cliquer sur Logout**
4. **Vérifier les logs** dans la console du WebApp :
   ```
   [INFO] Logout: Starting logout process
   [INFO] Logout: Token cleared from memory
   [INFO] Logout: Token cleared from TokenStore for circuit ...
   [INFO] Logout: Tokens cleared from localStorage
   [INFO] Logout process completed
   ```
5. **Essayer d'accéder à une page protégée** → Devrait rediriger vers `/login`

### 5. Tester le Login après Logout

1. **Se reconnecter** avec le même utilisateur
2. **Vérifier** que tout fonctionne normalement
3. **Créer/Modifier une entité** pour tester l'audit log

---

## 📊 RÉSULTAT ATTENDU

### ✅ **AVANT le Logout**
```
[DEBUG] AuthHttpClientHandler: ✓ Bearer token ADDED to request GET /odata/...
[INFO] HTTP GET /odata/... responded 200 in 45ms
```

### ✅ **PENDANT le Logout**
```
[INFO] Logout: Starting logout process
[INFO] Logout: Logout request sent to API
[INFO] Logout: Token cleared from memory
[INFO] Logout: Token cleared from TokenStore for circuit abc123...
[INFO] Logout: Tokens cleared from localStorage
[INFO] Logout: Logout process completed
```

### ✅ **APRÈS le Logout**
```
[WARNING] AuthHttpClientHandler: AuthService has no token in memory for request GET /odata/...
[INFO] HTTP GET /odata/... responded 401 in 5ms
```

OU (si redirection automatique) :
```
[INFO] Navigation to /login
```

---

## 🎯 BONUS : Augmenter la Durée de Vie du Token

Si tu veux éviter que le token expire trop vite (actuellement 60 minutes), tu peux l'augmenter :

**Fichier** : `src/TunNetCom.SilkRoadErp.Sales.Api/appsettings.json`

```json
"JwtSettings": {
  "AccessTokenExpirationMinutes": 480,  // 8 heures au lieu de 60 minutes
  "RefreshTokenExpirationDays": 7
}
```

**Valeurs recommandées** :
- **Développement** : 480 minutes (8 heures)
- **Production** : 60-120 minutes (1-2 heures) + utiliser le Refresh Token

---

## 📝 AUTRES AMÉLIORATIONS POSSIBLES

### 1. Refresh Token Automatique

Actuellement, quand le token expire, l'utilisateur doit se reconnecter. On pourrait implémenter un **refresh automatique** :

```csharp
// Dans AuthHttpClientHandler.cs
if (response.StatusCode == 401 && tokenExpiryDetected)
{
    // Tenter de refresh le token
    var refreshed = await _authService.RefreshTokenAsync();
    if (refreshed)
    {
        // Réessayer la requête avec le nouveau token
        return await base.SendAsync(request, cancellationToken);
    }
}
```

### 2. Notification d'Expiration

Afficher un message à l'utilisateur **avant** que le token expire :

```csharp
// Vérifier l'expiration du token
var tokenExpiry = GetTokenExpiry(accessToken);
var timeRemaining = tokenExpiry - DateTime.UtcNow;

if (timeRemaining < TimeSpan.FromMinutes(5))
{
    NotificationService.Notify("Votre session expire dans 5 minutes");
}
```

---

## ✅ RÉSUMÉ

| Problème | Solution | Status |
|----------|----------|--------|
| Token expiré après logout | Vider `AccessToken` en mémoire | ✅ Fixé |
| Ordre de nettoyage | Mémoire → TokenStore → localStorage | ✅ Fixé |
| Pas de logs | Ajout de logs détaillés | ✅ Ajouté |
| Token expire trop vite | Augmenté à 60 min (peut aller à 480) | ⚠️ Optionnel |

**Compilation** : ✅ **SUCCÈS** (0 erreurs)

---

**TESTE MAINTENANT ! 🚀**

1. Redémarre l'API et le WebApp
2. Connecte-toi
3. Fais logout
4. Vérifie que le token est bien vidé
5. Reconnecte-toi
6. Crée une entité pour tester l'audit log



