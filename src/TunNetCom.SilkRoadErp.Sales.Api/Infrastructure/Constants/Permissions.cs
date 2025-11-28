namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

/// <summary>
/// Constantes pour toutes les permissions de l'application
/// </summary>
public static class Permissions
{
    // ==================== INVOICES (FACTURES) ====================
    public const string ViewInvoices = "CanViewInvoices";
    public const string CreateInvoice = "CanCreateInvoice";
    public const string UpdateInvoice = "CanUpdateInvoice";
    public const string DeleteInvoice = "CanDeleteInvoice";
    public const string ExportInvoices = "CanExportInvoices";

    // ==================== DELIVERY NOTES (BONS DE LIVRAISON) ====================
    public const string ViewDeliveryNotes = "CanViewDeliveryNotes";
    public const string CreateDeliveryNote = "CanCreateDeliveryNote";
    public const string UpdateDeliveryNote = "CanUpdateDeliveryNote";
    public const string DeleteDeliveryNote = "CanDeleteDeliveryNote";
    public const string AttachDeliveryNoteToInvoice = "CanAttachDeliveryNoteToInvoice";
    public const string DetachDeliveryNoteFromInvoice = "CanDetachDeliveryNoteFromInvoice";

    // ==================== PRICE QUOTES (DEVIS) ====================
    public const string ViewPriceQuotes = "CanViewPriceQuotes";
    public const string CreatePriceQuote = "CanCreatePriceQuote";
    public const string UpdatePriceQuote = "CanUpdatePriceQuote";
    public const string DeletePriceQuote = "CanDeletePriceQuote";

    // ==================== ORDERS (COMMANDES) ====================
    public const string ViewOrders = "CanViewOrders";
    public const string CreateOrder = "CanCreateOrder";
    public const string UpdateOrder = "CanUpdateOrder";
    public const string DeleteOrder = "CanDeleteOrder";

    // ==================== PRODUCTS (PRODUITS) ====================
    public const string ViewProducts = "CanViewProducts";
    public const string CreateProduct = "CanCreateProduct";
    public const string UpdateProduct = "CanUpdateProduct";
    public const string DeleteProduct = "CanDeleteProduct";
    public const string ViewProductStock = "CanViewProductStock";

    // ==================== CUSTOMERS (CLIENTS) ====================
    public const string ViewCustomers = "CanViewCustomers";
    public const string CreateCustomer = "CanCreateCustomer";
    public const string UpdateCustomer = "CanUpdateCustomer";
    public const string DeleteCustomer = "CanDeleteCustomer";

    // ==================== PROVIDERS (FOURNISSEURS) ====================
    public const string ViewProviders = "CanViewProviders";
    public const string CreateProvider = "CanCreateProvider";
    public const string UpdateProvider = "CanUpdateProvider";
    public const string DeleteProvider = "CanDeleteProvider";

    // ==================== RECEIPT NOTES (BONS DE RÉCEPTION) ====================
    public const string ViewReceiptNotes = "CanViewReceiptNotes";
    public const string CreateReceiptNote = "CanCreateReceiptNote";
    public const string UpdateReceiptNote = "CanUpdateReceiptNote";
    public const string DeleteReceiptNote = "CanDeleteReceiptNote";
    public const string AttachReceiptNoteToInvoice = "CanAttachReceiptNoteToInvoice";
    public const string DetachReceiptNoteFromInvoice = "CanDetachReceiptNoteFromInvoice";

    // ==================== PROVIDER INVOICES (FACTURES FOURNISSEURS) ====================
    public const string ViewProviderInvoices = "CanViewProviderInvoices";
    public const string CreateProviderInvoice = "CanCreateProviderInvoice";
    public const string UpdateProviderInvoice = "CanUpdateProviderInvoice";
    public const string DeleteProviderInvoice = "CanDeleteProviderInvoice";
    public const string ExportProviderInvoices = "CanExportProviderInvoices";

    // ==================== AVOIRS (CREDIT NOTES) ====================
    public const string ViewAvoirs = "CanViewAvoirs";
    public const string CreateAvoir = "CanCreateAvoir";
    public const string UpdateAvoir = "CanUpdateAvoir";
    public const string DeleteAvoir = "CanDeleteAvoir";

    // ==================== AVOIRS FOURNISSEURS (SUPPLIER CREDIT NOTES) ====================
    public const string ViewAvoirsFournisseur = "CanViewAvoirsFournisseur";
    public const string CreateAvoirFournisseur = "CanCreateAvoirFournisseur";
    public const string UpdateAvoirFournisseur = "CanUpdateAvoirFournisseur";
    public const string DeleteAvoirFournisseur = "CanDeleteAvoirFournisseur";

    // ==================== FACTURE AVOIR FOURNISSEUR ====================
    public const string ViewFactureAvoirFournisseur = "CanViewFactureAvoirFournisseur";
    public const string CreateFactureAvoirFournisseur = "CanCreateFactureAvoirFournisseur";
    public const string UpdateFactureAvoirFournisseur = "CanUpdateFactureAvoirFournisseur";
    public const string DeleteFactureAvoirFournisseur = "CanDeleteFactureAvoirFournisseur";

    // ==================== PAYMENTS (PAIEMENTS) ====================
    public const string ViewPaymentsClient = "CanViewPaymentsClient";
    public const string CreatePaymentClient = "CanCreatePaymentClient";
    public const string UpdatePaymentClient = "CanUpdatePaymentClient";
    public const string DeletePaymentClient = "CanDeletePaymentClient";

    public const string ViewPaymentsFournisseur = "CanViewPaymentsFournisseur";
    public const string CreatePaymentFournisseur = "CanCreatePaymentFournisseur";
    public const string UpdatePaymentFournisseur = "CanUpdatePaymentFournisseur";
    public const string DeletePaymentFournisseur = "CanDeletePaymentFournisseur";

    // ==================== INVENTORY (INVENTAIRE) ====================
    public const string ViewInventory = "CanViewInventory";
    public const string CreateInventory = "CanCreateInventory";
    public const string UpdateInventory = "CanUpdateInventory";
    public const string DeleteInventory = "CanDeleteInventory";
    public const string ValidateInventory = "CanValidateInventory";
    public const string CloseInventory = "CanCloseInventory";

    // ==================== BANKS (BANQUES) ====================
    public const string ViewBanks = "CanViewBanks";
    public const string CreateBank = "CanCreateBank";
    public const string UpdateBank = "CanUpdateBank";
    public const string DeleteBank = "CanDeleteBank";

    // ==================== TAGS ====================
    public const string ViewTags = "CanViewTags";
    public const string CreateTag = "CanCreateTag";
    public const string UpdateTag = "CanUpdateTag";
    public const string DeleteTag = "CanDeleteTag";
    public const string ManageDocumentTags = "CanManageDocumentTags";

    // ==================== ACCOUNTING YEAR (EXERCICE COMPTABLE) ====================
    public const string ViewAccountingYear = "CanViewAccountingYear";
    public const string ManageAccountingYear = "CanManageAccountingYear";

    // ==================== APP PARAMETERS (PARAMÈTRES) ====================
    public const string ViewAppParameters = "CanViewAppParameters";
    public const string UpdateAppParameters = "CanUpdateAppParameters";

    // ==================== REPORTS (RAPPORTS) ====================
    public const string ViewSoldes = "CanViewSoldes";
    public const string ViewReports = "CanViewReports";
    public const string ExportData = "CanExportData";

    // ==================== USER MANAGEMENT ====================
    public const string ViewUsers = "CanViewUsers";
    public const string CreateUser = "CanCreateUser";
    public const string UpdateUser = "CanUpdateUser";
    public const string DeleteUser = "CanDeleteUser";
    public const string ManageUsers = "CanManageUsers";

    // ==================== ROLE MANAGEMENT ====================
    public const string ViewRoles = "CanViewRoles";
    public const string CreateRole = "CanCreateRole";
    public const string UpdateRole = "CanUpdateRole";
    public const string DeleteRole = "CanDeleteRole";
    public const string ManageRoles = "CanManageRoles";

    // ==================== PERMISSION MANAGEMENT ====================
    public const string ViewPermissions = "CanViewPermissions";
    public const string ManagePermissions = "CanManagePermissions";

    /// <summary>
    /// Retourne toutes les permissions sous forme de liste
    /// </summary>
    public static List<(string Name, string Description)> GetAllPermissions()
    {
        return new List<(string, string)>
        {
            // Invoices
            (ViewInvoices, "Peut voir les factures"),
            (CreateInvoice, "Peut créer des factures"),
            (UpdateInvoice, "Peut modifier des factures"),
            (DeleteInvoice, "Peut supprimer des factures"),
            (ExportInvoices, "Peut exporter les factures vers Sage"),

            // Delivery Notes
            (ViewDeliveryNotes, "Peut voir les bons de livraison"),
            (CreateDeliveryNote, "Peut créer des bons de livraison"),
            (UpdateDeliveryNote, "Peut modifier des bons de livraison"),
            (DeleteDeliveryNote, "Peut supprimer des bons de livraison"),
            (AttachDeliveryNoteToInvoice, "Peut attacher des bons de livraison aux factures"),
            (DetachDeliveryNoteFromInvoice, "Peut détacher des bons de livraison des factures"),

            // Price Quotes
            (ViewPriceQuotes, "Peut voir les devis"),
            (CreatePriceQuote, "Peut créer des devis"),
            (UpdatePriceQuote, "Peut modifier des devis"),
            (DeletePriceQuote, "Peut supprimer des devis"),

            // Orders
            (ViewOrders, "Peut voir les commandes"),
            (CreateOrder, "Peut créer des commandes"),
            (UpdateOrder, "Peut modifier des commandes"),
            (DeleteOrder, "Peut supprimer des commandes"),

            // Products
            (ViewProducts, "Peut voir les produits"),
            (CreateProduct, "Peut créer des produits"),
            (UpdateProduct, "Peut modifier des produits"),
            (DeleteProduct, "Peut supprimer des produits"),
            (ViewProductStock, "Peut voir le stock des produits"),

            // Customers
            (ViewCustomers, "Peut voir les clients"),
            (CreateCustomer, "Peut créer des clients"),
            (UpdateCustomer, "Peut modifier des clients"),
            (DeleteCustomer, "Peut supprimer des clients"),

            // Providers
            (ViewProviders, "Peut voir les fournisseurs"),
            (CreateProvider, "Peut créer des fournisseurs"),
            (UpdateProvider, "Peut modifier des fournisseurs"),
            (DeleteProvider, "Peut supprimer des fournisseurs"),

            // Receipt Notes
            (ViewReceiptNotes, "Peut voir les bons de réception"),
            (CreateReceiptNote, "Peut créer des bons de réception"),
            (UpdateReceiptNote, "Peut modifier des bons de réception"),
            (DeleteReceiptNote, "Peut supprimer des bons de réception"),
            (AttachReceiptNoteToInvoice, "Peut attacher des bons de réception aux factures"),
            (DetachReceiptNoteFromInvoice, "Peut détacher des bons de réception des factures"),

            // Provider Invoices
            (ViewProviderInvoices, "Peut voir les factures fournisseurs"),
            (CreateProviderInvoice, "Peut créer des factures fournisseurs"),
            (UpdateProviderInvoice, "Peut modifier des factures fournisseurs"),
            (DeleteProviderInvoice, "Peut supprimer des factures fournisseurs"),
            (ExportProviderInvoices, "Peut exporter les factures fournisseurs vers Sage"),

            // Avoirs
            (ViewAvoirs, "Peut voir les avoirs clients"),
            (CreateAvoir, "Peut créer des avoirs clients"),
            (UpdateAvoir, "Peut modifier des avoirs clients"),
            (DeleteAvoir, "Peut supprimer des avoirs clients"),

            // Avoirs Fournisseur
            (ViewAvoirsFournisseur, "Peut voir les avoirs fournisseurs"),
            (CreateAvoirFournisseur, "Peut créer des avoirs fournisseurs"),
            (UpdateAvoirFournisseur, "Peut modifier des avoirs fournisseurs"),
            (DeleteAvoirFournisseur, "Peut supprimer des avoirs fournisseurs"),

            // Facture Avoir Fournisseur
            (ViewFactureAvoirFournisseur, "Peut voir les factures d'avoir fournisseur"),
            (CreateFactureAvoirFournisseur, "Peut créer des factures d'avoir fournisseur"),
            (UpdateFactureAvoirFournisseur, "Peut modifier des factures d'avoir fournisseur"),
            (DeleteFactureAvoirFournisseur, "Peut supprimer des factures d'avoir fournisseur"),

            // Payments Client
            (ViewPaymentsClient, "Peut voir les paiements clients"),
            (CreatePaymentClient, "Peut créer des paiements clients"),
            (UpdatePaymentClient, "Peut modifier des paiements clients"),
            (DeletePaymentClient, "Peut supprimer des paiements clients"),

            // Payments Fournisseur
            (ViewPaymentsFournisseur, "Peut voir les paiements fournisseurs"),
            (CreatePaymentFournisseur, "Peut créer des paiements fournisseurs"),
            (UpdatePaymentFournisseur, "Peut modifier des paiements fournisseurs"),
            (DeletePaymentFournisseur, "Peut supprimer des paiements fournisseurs"),

            // Inventory
            (ViewInventory, "Peut voir les inventaires"),
            (CreateInventory, "Peut créer des inventaires"),
            (UpdateInventory, "Peut modifier des inventaires"),
            (DeleteInventory, "Peut supprimer des inventaires"),
            (ValidateInventory, "Peut valider des inventaires"),
            (CloseInventory, "Peut clôturer des inventaires"),

            // Banks
            (ViewBanks, "Peut voir les banques"),
            (CreateBank, "Peut créer des banques"),
            (UpdateBank, "Peut modifier des banques"),
            (DeleteBank, "Peut supprimer des banques"),

            // Tags
            (ViewTags, "Peut voir les tags"),
            (CreateTag, "Peut créer des tags"),
            (UpdateTag, "Peut modifier des tags"),
            (DeleteTag, "Peut supprimer des tags"),
            (ManageDocumentTags, "Peut gérer les tags des documents"),

            // Accounting Year
            (ViewAccountingYear, "Peut voir les exercices comptables"),
            (ManageAccountingYear, "Peut gérer les exercices comptables"),

            // App Parameters
            (ViewAppParameters, "Peut voir les paramètres de l'application"),
            (UpdateAppParameters, "Peut modifier les paramètres de l'application"),

            // Reports
            (ViewSoldes, "Peut voir les soldes"),
            (ViewReports, "Peut voir les rapports"),
            (ExportData, "Peut exporter les données"),

            // User Management
            (ViewUsers, "Peut voir les utilisateurs"),
            (CreateUser, "Peut créer des utilisateurs"),
            (UpdateUser, "Peut modifier des utilisateurs"),
            (DeleteUser, "Peut supprimer des utilisateurs"),
            (ManageUsers, "Peut gérer les utilisateurs"),

            // Role Management
            (ViewRoles, "Peut voir les rôles"),
            (CreateRole, "Peut créer des rôles"),
            (UpdateRole, "Peut modifier des rôles"),
            (DeleteRole, "Peut supprimer des rôles"),
            (ManageRoles, "Peut gérer les rôles"),

            // Permission Management
            (ViewPermissions, "Peut voir les permissions"),
            (ManagePermissions, "Peut gérer les permissions")
        };
    }
}

