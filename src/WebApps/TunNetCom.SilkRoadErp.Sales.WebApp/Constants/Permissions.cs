namespace TunNetCom.SilkRoadErp.Sales.WebApp.Constants;

/// <summary>
/// Constantes pour toutes les permissions de l'application (côté front-end)
/// IMPORTANT: Ces valeurs doivent correspondre exactement à celles du backend
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
}

