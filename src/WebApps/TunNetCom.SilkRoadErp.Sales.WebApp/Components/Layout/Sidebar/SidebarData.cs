namespace BlazorProTemplate.Components.Layout.Sidebar;

public class SidebarData
{

    public static List<MenuItem> GetStandardMenuItems() => new()
    {
        new MenuItem(href:"/", title: "Home", icon:"ri-home-3-fill"),
        new MenuItem(href:"/dashboard", title: "Dashboard", icon:"ri-dashboard-fill"),
        //new MenuItem(href:"/weather", title: "Weather", icon:"ri-bar-chart-horizontal-line"),
    };

    public static List<MenuItem> GetGeneralMenuItems() => new()
    {
        new(title:"Clients", icon:"ri-team-fill",  childMenuItems:
        [
            new MenuItem(href:"/customers_list", title:"Consulter Clients"),
            new MenuItem(title:"Facturation", childMenuItems:
            [
                new MenuItem(href:"/manage-invoice", title:"Gestion Facture"),

                new MenuItem(title:"Bon de livraison", childMenuItems:
                [
                   new MenuItem(href:"/addOrUpdateDeliveryNote", title:"Gestion Bons de Livraison"),
                   new MenuItem(href:"/delivery-notes", title:"Consulter Bons de Livraison"),
                   new MenuItem(href:"/delivery-notes", title:"Devis"),
                   new MenuItem(title:"More", childMenuItems:
                   [
                      
                   ]),
                ]),
                new MenuItem(href:"/delivery-notes", title:"Devis"),

            ]),
        ]),

        new MenuItem(title:"Fournisseurs", icon:"ri-store-2-fill", childMenuItems:
        [
            new MenuItem(href:"/providers_list", title:"Consulter Fournisseurs"),
            new MenuItem(href:"/manage-providers-invoices", title:"Gestion Facture Fournisseurs"),
            new MenuItem(href:"#", title:"Commandes Fournisseurs"),
        ]),

        //new MenuItem(title:"Stock", icon:"ri-shopping-cart-fill", childMenuItems:
        //[
        //    new MenuItem(href:"#", title:"Consulter Stock"),
        //]),

        new MenuItem(title:"Stock", icon:"ri-shopping-cart-fill", childMenuItems:
        [
            new MenuItem(href:"/products_list", title:"Stock"),
            new MenuItem(href:"#", title:"Open street map"),
        ]),
        new MenuItem(title:"Theme", icon:"ri-paint-brush-fill", childMenuItems:
        [
            new MenuItem(href:"#", title:"Dark"),
            new MenuItem(href:"#", title:"Light"),
        ]),
    };

}
