# Audit des Endpoints - Permissions à Ajouter

## ✅ Endpoints AVEC Permissions (8 endpoints sur 116)

1. ✅ `Invoices/CreateInvoice` - `Permission:CanCreateInvoice`
2. ✅ `DeliveryNote/CreateDeliveryNote` - `Permission:CanCreateDeliveryNote`
3. ✅ `priceQuote/CreatePriceQuote` - `Permission:CanCreatePriceQuote`
4. ✅ `Commandes/CreateOrder` - `Permission:CanCreateOrder`
5. ✅ `Products/CreateProduct` - `Permission:CanCreateProduct` (modifié avec constante)
6. ✅ `Providers/CreateProvider` - `Permission:CanCreateProvider`
7. ✅ `Customers/CreateCustomer` - `Permission:CanCreateCustomer`
8. ✅ `Auth/Logout` - `RequireAuthorization()`

## ❌ Endpoints SANS Permissions (108 endpoints sur 116)

### INVOICES (5 endpoints à sécuriser)
- `Invoices/GetInvoicesWithSummaries` → `ViewInvoices`
- `Invoices/GetInvoicesWithIds` → `ViewInvoices`
- `Invoices/GetInvoicesByCustomerWithSummary` → `ViewInvoices`
- `Invoices/GetFullInvoiceById` → `ViewInvoices`
- `Invoices/ExportToSageErp` → `ExportInvoices`

### DELIVERY NOTES (11 endpoints à sécuriser)
- `DeliveryNote/UpdateDeliveryNote` → `UpdateDeliveryNote`
- `DeliveryNote/DeleteDeliveryNote` → `DeleteDeliveryNote`
- `DeliveryNote/GetDeliveryNote` → `ViewDeliveryNotes`
- `DeliveryNote/GetDeliveryNoteByNum` → `ViewDeliveryNotes`
- `DeliveryNote/GetDeliveryNotesBasedOnProductReference` → `ViewDeliveryNotes`
- `DeliveryNote/GetDeliveryNotesBaseInfosWithSummaries` → `ViewDeliveryNotes`
- `DeliveryNote/GetDeliveryNotesByClientId` → `ViewDeliveryNotes`
- `DeliveryNote/GetDeliveryNotesByInvoiceId` → `ViewDeliveryNotes`
- `DeliveryNote/GetUninvoicedDeliveryNotes` → `ViewDeliveryNotes`
- `DeliveryNote/AttachToInvoice` → `AttachDeliveryNoteToInvoice`
- `DeliveryNote/DetachFromInvoice` → `DetachDeliveryNoteFromInvoice`

### PRICE QUOTES (4 endpoints à sécuriser)
- `priceQuote/UpdatePriceQuote` → `UpdatePriceQuote`
- `priceQuote/DeletePriceQuote` → `DeletePriceQuote`
- `priceQuote/GetPriceQuote` → `ViewPriceQuotes`
- `priceQuote/GetPriceQuoteById` → `ViewPriceQuotes`

### ORDERS (3 endpoints à sécuriser)
- `Commandes/UpdateOrder` → `UpdateOrder`
- `Commandes/GetCommandes` → `ViewOrders`
- `Commandes/GetCommande` → `ViewOrders`

### PRODUCTS (0 endpoints restants - ✅ TOUS MODIFIÉS)
- ✅ Tous les endpoints Products ont été sécurisés

### CUSTOMERS (4 endpoints à sécuriser)
- `Customers/UpdateCustomer` → `UpdateCustomer`
- `Customers/DeleteCustomer` → `DeleteCustomer`
- `Customers/GetCustomer` → `ViewCustomers`
- `Customers/GetCustomerById` → `ViewCustomers`

### PROVIDERS (4 endpoints à sécuriser)
- `Providers/UpdateProvider` → `UpdateProvider`
- `Providers/DeleteProvider` → `DeleteProvider`
- `Providers/GetProvider` → `ViewProviders`
- `Providers/GetProviderById` → `ViewProviders`

### RECEIPT NOTES (10+ endpoints à sécuriser)
- `ReceiptNote/CreateReceiptNote` → `CreateReceiptNote`
- `ReceiptNote/UpdateReceiptNoteWithLines` → `UpdateReceiptNote`
- `ReceiptNote/DeleteReceiptNote` → `DeleteReceiptNote`
- `ReceiptNote/GetReceiptNoteWithDetails` → `ViewReceiptNotes`
- `ReceiptNote/GetReceiptNotesBasedOnProductReference` → `ViewReceiptNotes`
- `ReceiptNote/AttachToProviderInvoice` → `AttachReceiptNoteToInvoice`
- `ReceiptNote/DetachFromProviderInvoice` → `DetachReceiptNoteFromInvoice`
- ... (autres endpoints ReceiptNote)

### PROVIDER INVOICES (5+ endpoints à sécuriser)
- `ProviderInvoice/CreateProviderInvoice` → `CreateProviderInvoice`
- `ProviderInvoice/UpdateProviderInvoice` → `UpdateProviderInvoice`
- `ProviderInvoice/GetFullProviderInvoice` → `ViewProviderInvoices`
- `ProviderInvoice/GetProviderInvoicesWithIdsList` → `ViewProviderInvoices`
- `ProviderInvoices/ExportToSageErp` → `ExportProviderInvoices`

### AVOIRS (5 endpoints à sécuriser)
- `Avoirs/CreateAvoir` → `CreateAvoir`
- `Avoirs/UpdateAvoir` → `UpdateAvoir`
- `Avoirs/GetAvoir` → `ViewAvoirs`
- `Avoirs/GetFullAvoir` → `ViewAvoirs`
- `Avoirs/GetAvoirsWithSummaries` → `ViewAvoirs`

### AVOIRS FOURNISSEUR (5 endpoints à sécuriser)
- `AvoirFournisseur/CreateAvoirFournisseur` → `CreateAvoirFournisseur`
- `AvoirFournisseur/UpdateAvoirFournisseur` → `UpdateAvoirFournisseur`
- `AvoirFournisseur/GetAvoirFournisseur` → `ViewAvoirsFournisseur`
- `AvoirFournisseur/GetFullAvoirFournisseur` → `ViewAvoirsFournisseur`
- `AvoirFournisseur/GetAvoirFournisseurWithSummaries` → `ViewAvoirsFournisseur`

### FACTURE AVOIR FOURNISSEUR (5 endpoints à sécuriser)
- `FactureAvoirFournisseur/CreateFactureAvoirFournisseur` → `CreateFactureAvoirFournisseur`
- `FactureAvoirFournisseur/UpdateFactureAvoirFournisseur` → `UpdateFactureAvoirFournisseur`
- `FactureAvoirFournisseur/GetFactureAvoirFournisseur` → `ViewFactureAvoirFournisseur`
- `FactureAvoirFournisseur/GetFullFactureAvoirFournisseur` → `ViewFactureAvoirFournisseur`
- `FactureAvoirFournisseur/GetFactureAvoirFournisseurWithSummaries` → `ViewFactureAvoirFournisseur`

### PAYMENTS CLIENT (4 endpoints à sécuriser)
- `PaiementClient/CreatePaiementClient` → `CreatePaymentClient`
- `PaiementClient/UpdatePaiementClient` → `UpdatePaymentClient`
- `PaiementClient/DeletePaiementClient` → `DeletePaymentClient`
- `PaiementClient/GetPaiementClient` → `ViewPaymentsClient`
- `PaiementClient/GetPaiementsClient` → `ViewPaymentsClient`

### PAYMENTS FOURNISSEUR (4 endpoints à sécuriser)
- `PaiementFournisseur/CreatePaiementFournisseur` → `CreatePaymentFournisseur`
- `PaiementFournisseur/UpdatePaiementFournisseur` → `UpdatePaymentFournisseur`
- `PaiementFournisseur/DeletePaiementFournisseur` → `DeletePaymentFournisseur`
- `PaiementFournisseur/GetPaiementFournisseur` → `ViewPaymentsFournisseur`
- `PaiementFournisseur/GetPaiementsFournisseur` → `ViewPaymentsFournisseur`

### INVENTORY (9 endpoints à sécuriser)
- `Inventaire/CreateInventaire` → `CreateInventory`
- `Inventaire/UpdateInventaire` → `UpdateInventory`
- `Inventaire/DeleteInventaire` → `DeleteInventory`
- `Inventaire/GetInventaires` → `ViewInventory`
- `Inventaire/GetInventaireById` → `ViewInventory`
- `Inventaire/ValiderInventaire` → `ValidateInventory`
- `Inventaire/CloturerInventaire` → `CloseInventory`
- `Inventaire/GetDernierPrixAchat` → `ViewInventory`
- `Inventaire/GetHistoriqueAchatVente` → `ViewInventory`

### BANKS (2 endpoints à sécuriser)
- `Banque/CreateBanque` → `CreateBank`
- `Banque/GetBanques` → `ViewBanks`

### TAGS (8 endpoints à sécuriser)
- `Tags/CreateTag` → `CreateTag`
- `Tags/UpdateTag` → `UpdateTag`
- `Tags/DeleteTag` → `DeleteTag`
- `Tags/GetAllTags` → `ViewTags`
- `Tags/GetDocumentTags` → `ViewTags`
- `Tags/AddTagsToDocument` → `ManageDocumentTags`
- `Tags/AddTagsToDocumentByName` → `ManageDocumentTags`
- `Tags/RemoveTagsFromDocument` → `ManageDocumentTags`

### ACCOUNTING YEAR (3 endpoints à sécuriser)
- `AccountingYear/GetActiveAccountingYear` → `ViewAccountingYear`
- `AccountingYear/GetAllAccountingYears` → `ViewAccountingYear`
- `AccountingYear/SetActiveAccountingYear` → `ManageAccountingYear`

### APP PARAMETERS (2 endpoints à sécuriser)
- `AppParameters/GetAppParameters` → `ViewAppParameters`
- `AppParameters/UpdateAppParameters` → `UpdateAppParameters`

### SOLDES (2 endpoints à sécuriser)
- `Soldes/GetSoldeClient` → `ViewSoldes`
- `Soldes/GetSoldeFournisseur` → `ViewSoldes`

### AUTH (2 endpoints - pas besoin de permissions spécifiques)
- `Auth/Login` → ❌ PAS DE PERMISSION (public)
- `Auth/RefreshToken` → ❌ PAS DE PERMISSION (public)
- `Auth/Logout` → ✅ Requiert authentification

---

## 🎯 PLAN D'ACTION

### Priorité 1 - Endpoints Critiques (CRUD de base)
1. ✅ Products - **TERMINÉ**
2. ⏳ Customers - En cours
3. ⏳ Providers - En cours
4. ⏳ Invoices - En cours
5. ⏳ DeliveryNotes - En cours

### Priorité 2 - Documents Financiers
6. PriceQuotes
7. Orders
8. ProviderInvoices
9. ReceiptNotes
10. Avoirs
11. AvoirsFournisseur
12. FactureAvoirFournisseur

### Priorité 3 - Payments & Reports
13. PaiementsClient
14. PaiementsFournisseur
15. Soldes

### Priorité 4 - Administration
16. Inventory
17. Banks
18. Tags
19. AccountingYear
20. AppParameters

---

## 📝 TEMPLATE DE MODIFICATION

Pour chaque endpoint, ajouter :

```csharp
.RequireAuthorization($"Permission:{Permissions.PERMISSION_NAME}")
```

Avant `.WithTags(...)`.

Exemple :
```csharp
app.MapGet("/customers", HandleGetCustomersAsync)
    .RequireAuthorization($"Permission:{Permissions.ViewCustomers}")  // ← AJOUTER
    .WithTags(EndpointTags.Customers);
```

