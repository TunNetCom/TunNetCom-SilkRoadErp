# Test de l'isolation des sessions entre appareils

## Contexte

Ce document décrit comment tester que le problème de session partagée entre appareils a été résolu.

## Problème résolu

**Avant** : Quand un utilisateur se connectait sur un PC, puis partageait le lien de l'application vers un téléphone ou un autre PC, ce dernier voyait la session du premier utilisateur au lieu d'être redirigé vers la page de login.

**Après** : Chaque appareil/navigateur a maintenant sa propre session isolée basée sur son localStorage local. Partager le lien redirige vers la page de login.

## Scénarios de test

### Test 1 : Session isolée par appareil (Test principal)

**Objectif** : Vérifier qu'un nouvel appareil est redirigé vers login même si un autre appareil est déjà connecté.

**Étapes** :
1. **Sur PC 1** :
   - Ouvrir l'application dans un navigateur
   - Se connecter avec un utilisateur (ex: `admin`)
   - Vérifier que l'application s'affiche correctement avec le nom d'utilisateur
   - Copier l'URL de l'application (ex: `http://localhost:5000/`)

2. **Sur PC 2 ou Téléphone** :
   - Ouvrir un NOUVEAU navigateur (ou mode incognito)
   - Coller l'URL copiée
   - **Résultat attendu** : L'application redirige automatiquement vers `/login`
   - **Résultat NON souhaité** : L'application affiche le compte `admin` directement

3. **Vérification** :
   - Sur PC 2, se connecter avec un autre utilisateur (ex: `user2`)
   - Vérifier que PC 1 reste connecté avec `admin`
   - Vérifier que PC 2 est connecté avec `user2`
   - Les deux sessions sont indépendantes ✅

### Test 2 : Même navigateur, onglets multiples

**Objectif** : Vérifier que plusieurs onglets dans le même navigateur partagent la session (comportement normal).

**Étapes** :
1. Se connecter sur un onglet
2. Ouvrir un nouvel onglet dans le même navigateur
3. Naviguer vers l'application
4. **Résultat attendu** : L'utilisateur est déjà connecté (même localStorage)

### Test 3 : Déconnexion sur un appareil n'affecte pas l'autre

**Objectif** : Vérifier que chaque appareil a sa propre session côté client.

**Étapes** :
1. Connecter User A sur PC 1
2. Connecter User B sur PC 2
3. Déconnecter sur PC 1
4. **Résultat attendu** : PC 2 reste connecté avec User B

### Test 4 : Partage de lien vers une page protégée

**Objectif** : Vérifier que partager un lien vers une page interne redirige vers login avec returnUrl.

**Étapes** :
1. Sur PC 1, se connecter et naviguer vers `/products`
2. Copier l'URL `http://localhost:5000/products`
3. Sur PC 2 (nouveau navigateur), coller l'URL
4. **Résultat attendu** : 
   - Redirection vers `/login?returnUrl=http%3A%2F%2Flocalhost%3A5000%2Fproducts`
   - Après connexion, redirection automatique vers `/products`

### Test 5 : Reconnexion après fermeture du navigateur

**Objectif** : Vérifier que la session persiste via localStorage même après fermeture.

**Étapes** :
1. Se connecter sur PC
2. Fermer complètement le navigateur
3. Rouvrir le navigateur et naviguer vers l'application
4. **Résultat attendu** : L'utilisateur est toujours connecté (token chargé depuis localStorage)

## Logs à vérifier

Pendant les tests, surveillez les logs du serveur. Vous devriez voir :

**Lors d'un nouveau circuit (nouvel appareil)** :
```
AuthorizeRouteView: Starting authentication check for new circuit
AuthorizeRouteView: Vérification de l'authentification depuis localStorage...
LoadTokenFromStorageAsync: No token found in localStorage
AuthorizeRouteView: No authentication token found in localStorage for this device, redirecting to login
```

**Lors d'une connexion réussie** :
```
Login successful for user {username}
LoadTokenFromStorageAsync: Token loaded successfully for circuit {circuitId}
AuthorizeRouteView: User authenticated from localStorage, granting access
```

## Architecture technique

### Flux d'authentification corrigé

```
┌─────────────┐          ┌──────────────┐          ┌─────────────┐
│   PC 1      │          │   Serveur    │          │   PC 2      │
│  (Chrome)   │          │   Blazor     │          │  (Firefox)  │
└──────┬──────┘          └──────┬───────┘          └──────┬──────┘
       │                        │                         │
       │ Login (user X)         │                         │
       ├───────────────────────>│                         │
       │                        │                         │
       │ JWT Token              │                         │
       │<───────────────────────┤                         │
       │                        │                         │
  ┌────▼─────┐                 │                         │
  │localStorage                │                         │
  │PC1        │                │                         │
  │Token X    │                │                         │
  └───────────┘                │                         │
       │                        │                         │
       │                        │   Ouvre l'app           │
       │                        │<────────────────────────┤
       │                        │                         │
       │                        │   Charge localStorage   │
       │                        │   de PC2                │
       │                        │                    ┌────▼─────┐
       │                        │                    │localStorage
       │                        │                    │PC2        │
       │                        │                    │Vide !     │
       │                        │                    └───────────┘
       │                        │                         │
       │                        │   Redirect /login       │
       │                        ├────────────────────────>│
       │                        │                         │
```

### Modifications clés

1. **AuthService.cs** : 
   - Suppression de la clé globale `GlobalTokenStoreKey`
   - Utilisation de `CircuitIdService` pour des clés par circuit
   - localStorage est la source de vérité unique

2. **JwtAuthenticationStateProvider.cs** :
   - Charge systématiquement depuis localStorage au premier appel
   - Pas de cache inter-circuits

3. **AuthorizeRouteView.razor** :
   - Force le chargement depuis localStorage pour chaque nouveau circuit
   - Redirige vers login si aucun token trouvé

## Tests automatisés (optionnel)

Pour des tests automatisés, vous pourriez utiliser :
- **Playwright** ou **Selenium** pour simuler plusieurs navigateurs
- Vérifier que l'URL change vers `/login` quand on ouvre l'app sans token

## Résultat attendu global

✅ Chaque navigateur/appareil a sa propre session indépendante  
✅ Partager le lien ne partage plus l'authentification  
✅ Un utilisateur peut être connecté simultanément sur plusieurs appareils  
✅ La déconnexion sur un appareil n'affecte pas les autres  
✅ Comportement standard des applications web modernes

## Remarques

- Le `TokenStore` est conservé comme cache de performance par circuit
- Chaque circuit utilise maintenant une clé unique basée sur son ID
- Le localStorage reste la source de vérité pour l'authentification
- Les sessions côté serveur (SignalR circuits) sont maintenant correctement isolées

