# Correction du problème de session partagée entre appareils

## Date
22 Décembre 2025

## Problème identifié

L'application Blazor Server utilisait un `TokenStore` singleton avec une clé globale fixe (`"global_access_token"`), ce qui causait le partage des tokens JWT entre tous les circuits Blazor Server, et donc entre tous les appareils/navigateurs.

### Symptôme
Quand un utilisateur se connectait sur un PC, puis partageait le lien de l'application vers un téléphone ou un autre PC, le second appareil voyait automatiquement la session du premier utilisateur au lieu d'être redirigé vers la page de login.

### Cause racine
- `TokenStore` déclaré comme **singleton** dans `Program.cs`
- `AuthService` utilisait une clé fixe `"global_access_token"` pour stocker/récupérer les tokens
- Tous les circuits partageaient le même dictionnaire en mémoire côté serveur
- Le localStorage du navigateur n'était pas utilisé comme source de vérité

## Solution implémentée

### Changements architecturaux

1. **localStorage comme source de vérité unique**
   - Chaque navigateur/appareil a son propre localStorage isolé
   - Les tokens ne sont plus partagés via un store serveur global
   - Le `TokenStore` est conservé uniquement comme cache de performance par circuit

2. **Clés spécifiques par circuit**
   - Utilisation de `CircuitIdService` pour générer des clés uniques par circuit
   - Chaque circuit Blazor a maintenant son propre cache isolé
   - Élimination de la constante `GlobalTokenStoreKey`

### Fichiers modifiés

#### 1. `AuthService.cs`
**Localisation** : `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/AuthService.cs`

**Changements** :
- Ajout de l'injection de `ICircuitIdService` dans le constructeur
- Suppression de la constante `GlobalTokenStoreKey = "global_access_token"`
- Suppression du chargement du token depuis `TokenStore` dans le constructeur
- Modification du getter `AccessToken` pour retourner uniquement `_localAccessToken` (pas de fallback vers TokenStore)
- Modification du setter `AccessToken` pour utiliser `_circuitIdService.GetCircuitId()` comme clé
- Modification de `SetAccessToken()` pour utiliser une clé par circuit
- Modification de `LoadTokenFromStorageAsync()` pour logger l'ID du circuit
- Modification de `LogoutAsync()` pour nettoyer le token avec la clé du circuit

**Impact** :
```csharp
// AVANT
private const string GlobalTokenStoreKey = "global_access_token";
_tokenStore.SetToken(GlobalTokenStoreKey, token);

// APRÈS
var circuitId = _circuitIdService.GetCircuitId();
_tokenStore.SetToken(circuitId, token);
```

#### 2. `JwtAuthenticationStateProvider.cs`
**Localisation** : `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/JwtAuthenticationStateProvider.cs`

**Changements** :
- Suppression du système de cache basé sur le temps (`_lastTokenLoadTime` et `_tokenLoadCacheDuration`)
- Ajout d'un flag simple `_hasLoadedFromStorage` pour forcer le chargement au premier appel
- Modification de `GetAuthenticationStateAsync()` pour toujours charger depuis localStorage au premier appel
- Modification de `NotifyAuthenticationStateChanged()` pour réinitialiser le flag

**Impact** :
```csharp
// AVANT - Cache temporel global
if (string.IsNullOrEmpty(token) && timeSinceLastLoad > _tokenLoadCacheDuration)

// APRÈS - Chargement systématique au premier appel du circuit
if (!_hasLoadedFromStorage)
{
    await _authService.LoadTokenFromStorageAsync();
    _hasLoadedFromStorage = true;
}
```

#### 3. `AuthorizeRouteView.razor`
**Localisation** : `src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Components/AuthorizeRouteView.razor`

**Changements** :
- Amélioration des commentaires dans `CheckAuthenticationAsync()` pour clarifier l'isolation
- Amélioration des messages de log pour mentionner le "nouveau circuit" et le "device"
- Messages utilisateur plus explicites ("Vérification de l'authentification depuis localStorage...")
- Logs plus détaillés pour diagnostiquer les problèmes d'isolation

**Impact** :
```csharp
// Messages de log améliorés
Logger.LogInformation("AuthorizeRouteView: Starting authentication check for new circuit");
Logger.LogWarning("AuthorizeRouteView: No authentication token found in localStorage for this device, redirecting to login");
```

## Flux d'authentification après correction

### Scénario : Utilisateur 1 se connecte, puis Utilisateur 2 ouvre l'app sur un autre appareil

1. **Utilisateur 1 se connecte sur PC** :
   ```
   Login → Backend génère JWT → Frontend stocke dans localStorage du PC
   Frontend cache aussi dans TokenStore avec circuitId1
   ```

2. **Utilisateur 2 ouvre l'app sur Phone** :
   ```
   Nouveau circuit (circuitId2) créé
   AuthorizeRouteView → LoadTokenFromStorageAsync()
   Tente de lire localStorage du Phone → Vide !
   AuthService.IsAuthenticated → false
   Redirection vers /login ✅
   ```

### Isolation garantie

```
┌────────────────┐        ┌────────────────┐
│   PC (User 1)  │        │  Phone (User 2)│
├────────────────┤        ├────────────────┤
│ localStorage   │        │ localStorage   │
│ Token: JWT_1   │        │ Token: (vide)  │
└────────┬───────┘        └────────┬───────┘
         │                         │
         └────────┐       ┐────────┘
                  │       │
         ┌────────▼───────▼────────┐
         │   Serveur Blazor        │
         ├─────────────────────────┤
         │ TokenStore (Singleton)  │
         │ ┌─────────────────────┐ │
         │ │ circuitId1: JWT_1   │ │
         │ │ circuitId2: (vide)  │ │
         │ └─────────────────────┘ │
         └─────────────────────────┘
```

## Avantages de la solution

✅ **Isolation complète** : Chaque appareil/navigateur a sa propre session  
✅ **Multi-appareils** : Un utilisateur peut être connecté simultanément sur plusieurs appareils  
✅ **Sécurité** : Partager un lien ne partage plus l'authentification  
✅ **Performance** : Le TokenStore reste comme cache local par circuit  
✅ **Standard** : Comportement conforme aux applications web modernes  
✅ **Pas de breaking changes** : L'API backend n'a pas été modifiée  

## Tests recommandés

Voir le document [`TESTING_SESSION_ISOLATION.md`](TESTING_SESSION_ISOLATION.md) pour les scénarios de test détaillés.

**Test principal** :
1. Se connecter sur un navigateur
2. Copier l'URL de l'application
3. Ouvrir l'URL dans un autre navigateur (ou mode incognito)
4. ✅ Vérifier la redirection vers `/login`

## Notes techniques

### Pourquoi conserver TokenStore ?
Le `TokenStore` est conservé comme cache de performance pour éviter les appels JS interop répétés au sein d'un même circuit. Maintenant, chaque circuit utilise sa propre clé (`circuitId`) au lieu d'une clé globale.

### CircuitIdService
`CircuitIdService` génère un ID unique par circuit Blazor Server basé sur :
1. Session HTTP (stable entre requêtes)
2. Connection ID (fallback)
3. GUID généré (dernier recours)

Cet ID garantit que chaque connexion SignalR a son propre espace de cache isolé.

### Compatibilité
- ✅ Compatible avec les tokens existants en localStorage
- ✅ Pas de migration de données nécessaire
- ✅ Les utilisateurs déjà connectés restent connectés
- ✅ Fonctionne avec le système de refresh token existant

## Surveillance et logs

Pour vérifier que la solution fonctionne, surveillez ces logs :

```
# Circuit 1 (PC)
CircuitIdService: Created new session-stored circuit ID: abc12345
LoadTokenFromStorageAsync: Token loaded successfully for circuit abc12345

# Circuit 2 (Phone) - Nouveau circuit sans token
CircuitIdService: Created new session-stored circuit ID: def67890
LoadTokenFromStorageAsync: No token found in localStorage
AuthorizeRouteView: No authentication token found in localStorage for this device, redirecting to login
```

## Prochaines étapes recommandées

1. ✅ Tester le scénario principal (PC → Phone)
2. ✅ Vérifier les logs pour confirmer l'isolation
3. ⚠️ Surveiller les performances (les appels localStorage peuvent être légèrement plus lents)
4. 💡 Envisager d'ajouter un indicateur visuel "Connecté sur X appareils" dans le futur

## Références

- [AUTH.md](AUTH.md) - Documentation complète du système d'authentification
- [TokenStore.cs](src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/TokenStore.cs) - Implémentation du cache
- [CircuitIdService.cs](src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Services/CircuitIdService.cs) - Génération des IDs uniques

