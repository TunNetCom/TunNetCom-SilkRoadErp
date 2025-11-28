# 🎯 Guide d'Utilisation des Permissions Côté Front-End

## 📋 Table des Matières
1. [Vue d'ensemble](#vue-densemble)
2. [Installation](#installation)
3. [Service de Permissions](#service-de-permissions)
4. [Composant HasPermission](#composant-haspermission)
5. [Exemples d'Utilisation](#exemples-dutilisation)
6. [Bonnes Pratiques](#bonnes-pratiques)

---

## Vue d'ensemble

Le système de permissions côté front-end permet de :
- ✅ **Cacher/Afficher** des boutons, liens, sections basés sur les permissions
- ✅ **Protéger l'accès** aux pages complètes
- ✅ **Vérifier les permissions** dans le code C#
- ✅ **Cache automatique** pour optimiser les performances
- ✅ **Synchronisation** avec les permissions backend

---

## Installation

### 1. Enregistrer le Service

Dans `Program.cs`, ajouter :

```csharp
// Ajouter le service de permissions
builder.Services.AddScoped<IPermissionService, PermissionService>();
```

**Placement** : Après l'enregistrement de `IAuthService`

```csharp
// Existing services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthHttpClientHandler>();

// ADD THIS LINE
builder.Services.AddScoped<IPermissionService, PermissionService>();  // ← NOUVEAU

// Continue with other services
builder.Services.AddSingleton<TokenStore>();
```

### 2. Fichiers Créés

✅ `Services/IPermissionService.cs` - Interface du service
✅ `Services/PermissionService.cs` - Implémentation du service
✅ `Components/Authorization/HasPermission.razor` - Composant Blazor
✅ `Constants/Permissions.cs` - Constantes des permissions (front-end)

---

## Service de Permissions

### Interface `IPermissionService`

```csharp
public interface IPermissionService
{
    // Vérifie UNE permission
    Task<bool> HasPermissionAsync(string permission);

    // Vérifie si l'utilisateur a AU MOINS UNE des permissions
    Task<bool> HasAnyPermissionAsync(params string[] permissions);

    // Vérifie si l'utilisateur a TOUTES les permissions
    Task<bool> HasAllPermissionsAsync(params string[] permissions);

    // Récupère toutes les permissions de l'utilisateur
    Task<IReadOnlyList<string>> GetUserPermissionsAsync();

    // Rafraîchit le cache des permissions
    Task RefreshPermissionsAsync();
}
```

### Utilisation dans le Code C#

```csharp
@inject IPermissionService PermissionService

@code {
    private bool canCreateInvoice = false;
    private bool canEditOrDelete = false;

    protected override async Task OnInitializedAsync()
    {
        // Vérifier UNE permission
        canCreateInvoice = await PermissionService.HasPermissionAsync(Permissions.CreateInvoice);

        // Vérifier plusieurs permissions (OU logique)
        canEditOrDelete = await PermissionService.HasAnyPermissionAsync(
            Permissions.UpdateInvoice, 
            Permissions.DeleteInvoice
        );

        // Vérifier plusieurs permissions (ET logique)
        var canManageAll = await PermissionService.HasAllPermissionsAsync(
            Permissions.CreateInvoice,
            Permissions.UpdateInvoice,
            Permissions.DeleteInvoice
        );
    }
}
```

---

## Composant HasPermission

### Syntaxe de Base

```razor
<HasPermission Permission="@Permissions.CreateInvoice">
    <button>Créer Facture</button>
</HasPermission>
```

### Paramètres

| Paramètre | Type | Description |
|-----------|------|-------------|
| `Permission` | `string` | UNE permission requise |
| `AnyPermissions` | `string[]` | L'utilisateur doit avoir AU MOINS UNE |
| `AllPermissions` | `string[]` | L'utilisateur doit avoir TOUTES |
| `ChildContent` | `RenderFragment` | Contenu affiché si permission OK |
| `FallbackContent` | `RenderFragment` | Contenu affiché si permission KO (optionnel) |

---

## Exemples d'Utilisation

### 1. Cacher un Bouton Basé sur une Permission

```razor
<HasPermission Permission="@Permissions.CreateInvoice">
    <RadzenButton Text="Nouvelle Facture" 
                  Icon="add" 
                  Click="@CreateInvoice" />
</HasPermission>
```

**Résultat** :
- ✅ L'utilisateur avec `CanCreateInvoice` → Bouton visible
- ❌ L'utilisateur sans permission → Rien ne s'affiche

---

### 2. Afficher un Message si Pas de Permission

```razor
<HasPermission Permission="@Permissions.DeleteInvoice">
    <ChildContent>
        <RadzenButton Text="Supprimer" 
                      ButtonStyle="ButtonStyle.Danger" 
                      Icon="delete" 
                      Click="@DeleteInvoice" />
    </ChildContent>
    <FallbackContent>
        <span class="text-muted">Vous n'avez pas les droits pour supprimer</span>
    </FallbackContent>
</HasPermission>
```

**Résultat** :
- ✅ Avec permission → Bouton "Supprimer"
- ❌ Sans permission → Message "Vous n'avez pas les droits..."

---

### 3. Vérifier Plusieurs Permissions (OU logique)

```razor
<HasPermission AnyPermissions="new[] { Permissions.UpdateInvoice, Permissions.DeleteInvoice }">
    <div class="action-buttons">
        <RadzenButton Text="Modifier" Icon="edit" />
        <RadzenButton Text="Supprimer" Icon="delete" />
    </div>
</HasPermission>
```

**Résultat** :
- ✅ L'utilisateur avec `CanUpdateInvoice` OU `CanDeleteInvoice` → Boutons visibles
- ❌ L'utilisateur sans aucune des deux → Rien

---

### 4. Vérifier Plusieurs Permissions (ET logique)

```razor
<HasPermission AllPermissions="new[] { Permissions.CreateInvoice, Permissions.ExportInvoices }">
    <RadzenButton Text="Créer et Exporter" Icon="cloud_download" />
</HasPermission>
```

**Résultat** :
- ✅ L'utilisateur avec `CanCreateInvoice` ET `CanExportInvoices` → Bouton visible
- ❌ L'utilisateur qui n'a pas les DEUX → Rien

---

### 5. Cacher une Section Complète

```razor
<RadzenCard>
    <h3>Gestion des Factures</h3>
    
    <!-- Liste des factures (tout le monde peut voir) -->
    <RadzenDataGrid Data="@invoices" />

    <!-- Actions (seulement avec permissions) -->
    <HasPermission AnyPermissions="new[] { 
        Permissions.CreateInvoice, 
        Permissions.UpdateInvoice, 
        Permissions.DeleteInvoice 
    }">
        <div class="actions">
            <HasPermission Permission="@Permissions.CreateInvoice">
                <RadzenButton Text="Créer" Icon="add" />
            </HasPermission>
            
            <HasPermission Permission="@Permissions.UpdateInvoice">
                <RadzenButton Text="Modifier" Icon="edit" />
            </HasPermission>
            
            <HasPermission Permission="@Permissions.DeleteInvoice">
                <RadzenButton Text="Supprimer" Icon="delete" />
            </HasPermission>
        </div>
    </HasPermission>
</RadzenCard>
```

---

### 6. Menu Sidebar Conditionnel

```razor
<RadzenPanelMenu>
    <!-- Toujours visible -->
    <RadzenPanelMenuItem Text="Tableau de bord" Icon="dashboard" Path="/" />

    <!-- Visible seulement avec permission -->
    <HasPermission Permission="@Permissions.ViewInvoices">
        <RadzenPanelMenuItem Text="Factures" Icon="receipt" Path="/invoices" />
    </HasPermission>

    <HasPermission Permission="@Permissions.ViewProducts">
        <RadzenPanelMenuItem Text="Produits" Icon="inventory" Path="/products" />
    </HasPermission>

    <HasPermission Permission="@Permissions.ViewCustomers">
        <RadzenPanelMenuItem Text="Clients" Icon="people" Path="/customers" />
    </HasPermission>

    <!-- Section Admin (plusieurs permissions requises) -->
    <HasPermission AnyPermissions="new[] { 
        Permissions.ManageUsers, 
        Permissions.ManageRoles 
    }">
        <RadzenPanelMenuItem Text="Administration" Icon="admin_panel_settings">
            <HasPermission Permission="@Permissions.ManageUsers">
                <RadzenPanelMenuItem Text="Utilisateurs" Icon="person" Path="/admin/users" />
            </HasPermission>
            
            <HasPermission Permission="@Permissions.ManageRoles">
                <RadzenPanelMenuItem Text="Rôles" Icon="security" Path="/admin/roles" />
            </HasPermission>
        </RadzenPanelMenuItem>
    </HasPermission>
</RadzenPanelMenu>
```

---

### 7. Protéger une Page Complète

```razor
@page "/invoices/create"
@inject IPermissionService PermissionService
@inject NavigationManager Navigation

<HasPermission Permission="@Permissions.CreateInvoice">
    <ChildContent>
        <h1>Créer une Nouvelle Facture</h1>
        <!-- Formulaire de création -->
        <EditForm Model="@invoice">
            <!-- ... -->
        </EditForm>
    </ChildContent>
    <FallbackContent>
        <RadzenCard>
            <h3>Accès Refusé</h3>
            <p>Vous n'avez pas la permission de créer des factures.</p>
            <RadzenButton Text="Retour" Click="@(() => Navigation.NavigateTo("/"))" />
        </RadzenCard>
    </FallbackContent>
</HasPermission>

@code {
    private InvoiceModel invoice = new();
}
```

**OU avec redirection automatique** :

```razor
@page "/invoices/create"
@inject IPermissionService PermissionService
@inject NavigationManager Navigation

@if (_hasPermission)
{
    <h1>Créer une Nouvelle Facture</h1>
    <!-- Formulaire -->
}

@code {
    private bool _hasPermission = false;

    protected override async Task OnInitializedAsync()
    {
        _hasPermission = await PermissionService.HasPermissionAsync(Permissions.CreateInvoice);
        
        if (!_hasPermission)
        {
            // Rediriger vers la page d'accueil
            Navigation.NavigateTo("/");
        }
    }
}
```

---

### 8. Vérifier dans une Méthode (avant d'appeler l'API)

```razor
@inject IPermissionService PermissionService
@inject NotificationService NotificationService

<RadzenButton Text="Supprimer" Click="@DeleteInvoiceAsync" />

@code {
    private async Task DeleteInvoiceAsync()
    {
        // Vérifier la permission AVANT d'appeler l'API
        if (!await PermissionService.HasPermissionAsync(Permissions.DeleteInvoice))
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = "Accès refusé",
                Detail = "Vous n'avez pas la permission de supprimer des factures.",
                Duration = 4000
            });
            return;
        }

        // Permission OK, procéder à la suppression
        await InvoiceService.DeleteAsync(invoiceId);
        NotificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = "Succès",
            Detail = "Facture supprimée avec succès.",
            Duration = 4000
        });
    }
}
```

---

### 9. DataGrid avec Actions Conditionnelles

```razor
<RadzenDataGrid Data="@invoices" TItem="InvoiceDto">
    <Columns>
        <RadzenDataGridColumn TItem="InvoiceDto" Property="Number" Title="Numéro" />
        <RadzenDataGridColumn TItem="InvoiceDto" Property="Date" Title="Date" />
        <RadzenDataGridColumn TItem="InvoiceDto" Property="TotalAmount" Title="Montant" />
        
        <!-- Colonne Actions -->
        <RadzenDataGridColumn TItem="InvoiceDto" Title="Actions" Width="200px">
            <Template Context="invoice">
                <div class="d-flex gap-2">
                    <!-- Bouton Voir (toujours visible) -->
                    <RadzenButton Icon="visibility" 
                                  ButtonStyle="ButtonStyle.Info" 
                                  Size="ButtonSize.Small"
                                  Click="@(() => ViewInvoice(invoice.Id))" />

                    <!-- Bouton Modifier (avec permission) -->
                    <HasPermission Permission="@Permissions.UpdateInvoice">
                        <RadzenButton Icon="edit" 
                                      ButtonStyle="ButtonStyle.Warning" 
                                      Size="ButtonSize.Small"
                                      Click="@(() => EditInvoice(invoice.Id))" />
                    </HasPermission>

                    <!-- Bouton Supprimer (avec permission) -->
                    <HasPermission Permission="@Permissions.DeleteInvoice">
                        <RadzenButton Icon="delete" 
                                      ButtonStyle="ButtonStyle.Danger" 
                                      Size="ButtonSize.Small"
                                      Click="@(() => DeleteInvoice(invoice.Id))" />
                    </HasPermission>

                    <!-- Bouton Exporter (avec permission) -->
                    <HasPermission Permission="@Permissions.ExportInvoices">
                        <RadzenButton Icon="download" 
                                      ButtonStyle="ButtonStyle.Success" 
                                      Size="ButtonSize.Small"
                                      Click="@(() => ExportInvoice(invoice.Id))" />
                    </HasPermission>
                </div>
            </Template>
        </RadzenDataGridColumn>
    </Columns>
</RadzenDataGrid>
```

---

## Bonnes Pratiques

### ✅ DO (À Faire)

1. **Toujours utiliser les constantes** `Permissions.XXX` au lieu de strings hardcodés
   ```csharp
   // ✅ BON
   Permission="@Permissions.CreateInvoice"
   
   // ❌ MAUVAIS
   Permission="CanCreateInvoice"
   ```

2. **Vérifier les permissions côté serveur** - Le front-end ne fait que cacher, l'API doit aussi vérifier
   ```csharp
   // Front-end cache le bouton
   // API vérifie avec [RequireAuthorization]
   ```

3. **Utiliser le cache** - Le `PermissionService` cache automatiquement pendant 5 minutes
   ```csharp
   // Pas besoin de cacher manuellement, le service le fait
   await PermissionService.HasPermissionAsync(permission);
   ```

4. **Rafraîchir après login/logout**
   ```csharp
   // Après un login réussi
   await PermissionService.RefreshPermissionsAsync();
   ```

### ❌ DON'T (À Éviter)

1. **Ne pas faire confiance uniquement au front-end** - Toujours vérifier côté API
2. **Ne pas hardcoder les permissions** - Utiliser les constantes
3. **Ne pas oublier de mettre à jour les deux fichiers `Permissions.cs`** (Backend + Frontend)
4. **Ne pas imbriquer trop de `<HasPermission>`** - Ça devient illisible

---

## 🎯 Checklist pour Ajouter une Nouvelle Permission

1. ✅ Ajouter la constante dans `Backend/Constants/Permissions.cs`
2. ✅ Ajouter la même constante dans `Frontend/Constants/Permissions.cs`
3. ✅ Ajouter la permission dans `Permissions.GetAllPermissions()` (Backend)
4. ✅ Ajouter `.RequireAuthorization($"Permission:{Permissions.XXX}")` sur l'endpoint API
5. ✅ Utiliser `<HasPermission Permission="@Permissions.XXX">` dans le composant Blazor
6. ✅ Tester avec différents rôles (Admin, Manager, User)

---

## 🔄 Synchronisation Backend ↔ Frontend

**IMPORTANT** : Les permissions définies dans le backend et le frontend **DOIVENT être identiques**.

### Backend
`src/TunNetCom.SilkRoadErp.Sales.Api/Infrastructure/Constants/Permissions.cs`

### Frontend
`src/WebApps/TunNetCom.SilkRoadErp.Sales.WebApp/Constants/Permissions.cs`

**Ces deux fichiers doivent avoir les MÊMES valeurs !**

---

## 🎓 Résumé Rapide

```razor
<!-- Cacher un bouton -->
<HasPermission Permission="@Permissions.CreateInvoice">
    <button>Créer</button>
</HasPermission>

<!-- Avec fallback -->
<HasPermission Permission="@Permissions.DeleteInvoice">
    <ChildContent><button>Supprimer</button></ChildContent>
    <FallbackContent><span>Pas de permission</span></FallbackContent>
</HasPermission>

<!-- Dans le code -->
@inject IPermissionService PermissionService

@code {
    var canCreate = await PermissionService.HasPermissionAsync(Permissions.CreateInvoice);
}
```

---

**Prochaines étapes** :
1. Enregistrer `IPermissionService` dans `Program.cs`
2. Mettre à jour le `JwtTokenService` backend pour inclure les permissions dans les claims JWT
3. Tester avec différents rôles

