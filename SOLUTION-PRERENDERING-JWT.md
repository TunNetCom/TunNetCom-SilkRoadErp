# Solution au problème de Prerendering et JWT

## Problème identifié

Les logs montrent clairement :

```
AuthHttpClientHandler: JS interop not available during prerendering for request POST /quotations
Message: JavaScript interop calls cannot be issued at this time. This is because the component is being statically rendered.
```

### Cause racine

Même si le prerendering est désactivé dans `App.razor` avec `prerender: false`, **certaines requêtes HTTP sont effectuées par les composants pendant leur phase d'initialisation**, **AVANT** que le circuit Blazor Server interactif soit établi et que JS interop soit disponible.

Pendant cette phase :
- `localStorage` n'est pas accessible
- `IJSRuntime` lance une `InvalidOperationException`
- Le token JWT ne peut pas être chargé
- Les requêtes HTTP sont envoyées SANS Authorization header

## Solutions possibles

### Solution 1 : Différer les appels API jusqu'après le premier render (RECOMMANDÉ)

Modifier les composants pour qu'ils n'appellent pas l'API dans `OnInitializedAsync` mais dans `OnAfterRenderAsync(firstRender: true)`.

**Exemple** :

```csharp
@code {
    private bool _hasLoaded = false;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_hasLoaded)
        {
            _hasLoaded = true;
            
            // Appeler l'API ici, après que JS interop soit disponible
            await LoadDataAsync();
            
            StateHasChanged();
        }
    }
    
    private async Task LoadDataAsync()
    {
        // Vos appels API ici
    }
}
```

**Avantages** :
- ✅ Simple à implémenter
- ✅ Pas de changement d'architecture
- ✅ Fonctionne avec le système actuel

**Inconvénients** :
- ❌ Nécessite de modifier tous les composants qui font des appels API
- ❌ Léger délai avant le chargement des données

### Solution 2 : Utiliser des Cookies HttpOnly au lieu de localStorage (MEILLEUR)

Stocker le JWT dans un cookie HttpOnly au lieu de `localStorage`.

**Avantages** :
- ✅ Le cookie est automatiquement envoyé avec chaque requête HTTP
- ✅ Plus sécurisé (protection XSS)
- ✅ Pas de problème de prerendering
- ✅ Pas besoin de JS interop

**Inconvénients** :
- ❌ Nécessite des modifications importantes (AuthService, API, AuthHttpClientHandler)
- ❌ Vulnérable aux attaques CSRF (nécessite un token anti-CSRF)

### Solution 3 : Utiliser un StateProvider côté serveur

Implémenter `AuthenticationStateProvider` pour stocker le token côté serveur dans la session Blazor.

**Avantages** :
- ✅ Approche recommandée par Microsoft pour Blazor Server
- ✅ Pas de problème de prerendering
- ✅ Token disponible immédiatement

**Inconvénients** :
- ❌ Changement d'architecture significatif
- ❌ Nécessite de refactoriser AuthService

## Solution immédiate (WORKAROUND)

En attendant une solution permanente, vous pouvez :

### Option A : Forcer un délai avant les appels API

Ajouter un petit délai dans les composants pour laisser le temps au circuit de s'établir :

```csharp
protected override async Task OnInitializedAsync()
{
    // Attendre que le circuit soit établi
    await Task.Delay(100);
    
    // Maintenant les appels API fonctionneront
    await LoadDataAsync();
}
```

### Option B : Retry automatique dans AuthHttpClientHandler

Modifier `AuthHttpClientHandler` pour réessayer de charger le token après un court délai si JS interop échoue.

### Option C : Désactiver complètement Blazor Server et utiliser Blazor WebAssembly

Si l'authentification est critique et que le prerendering cause trop de problèmes, envisager de migrer vers Blazor WebAssembly où `localStorage` est toujours accessible.

## Recommandation

**Pour une solution rapide (aujourd'hui)** : Utilisez **Solution 1** + **Option A** (différer les appels API).

**Pour une solution pérenne (cette semaine)** : Implémentez **Solution 2** (cookies HttpOnly).

## Implémentation rapide (Solution 1)

Voici un exemple de composant modifié pour différer les appels API :

```razor
@page "/quotations/add"
@inject IQuotationApiClient QuotationClient

<h3>Créer un devis</h3>

@if (!_isLoaded)
{
    <p>Chargement...</p>
}
else
{
    <!-- Votre formulaire ici -->
}

@code {
    private bool _isLoaded = false;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_isLoaded)
        {
            _isLoaded = true;
            await LoadInitialDataAsync();
            StateHasChanged();
        }
    }
    
    private async Task LoadInitialDataAsync()
    {
        // Charger les données nécessaires (clients, produits, etc.)
        // Ces appels auront maintenant accès au token JWT
    }
    
    private async Task CreateQuotationAsync()
    {
        // Créer le devis
        // Cet appel aura aussi accès au token JWT
    }
}
```

## Fichiers à modifier

Pour implémenter la Solution 1 rapidement :

1. Identifier tous les composants qui font des appels API dans `OnInitializedAsync`
2. Déplacer ces appels vers `OnAfterRenderAsync(firstRender: true)`
3. Ajouter un indicateur de chargement pendant le premier render

## Conclusion

Le problème n'est PAS dans `AuthHttpClientHandler` ou `AuthService`, mais dans le **timing** des appels API par rapport au cycle de vie des composants Blazor Server.

La solution la plus simple et rapide est de différer les appels API jusqu'après le premier render interactif.

