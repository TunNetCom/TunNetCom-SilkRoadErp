# SilkRoadErp

Welcome to SilkRoadErp, a comprehensive and efficient ERP solution designed to streamline your business processes. SilkRoadErp offers robust modules for managing your sales and purchasing operations, ensuring seamless integration and functionality. The application is built using a .NET backend and a Blazor Server frontend for a modern and responsive user experience.

## Table of Contents

- [Features](#features)
  - [Sales Module](#sales-module)
  - [Purchasing Module](#purchasing-module)
- [Technology Stack](#technology-stack)
- [Installation](#installation)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Features

### Sales Module

The Sales Module in SilkRoadErp includes:

- **Order Management**: Create, view, and manage sales orders.
- **Customer Management**: Maintain detailed records of customers and their interactions.
- **Invoice Generation**: Generate and send invoices to customers.
- **Sales Reporting**: Access comprehensive sales reports to track performance and trends.
- **Discount and Promotion Management**: Configure discounts and promotions for various products and services.

### Purchasing Module

The Purchasing Module in SilkRoadErp includes:

- **Supplier Management**: Maintain detailed records of suppliers and their products.
- **Purchase Order Management**: Create, view, and manage purchase orders.
- **Inventory Management**: Track and manage inventory levels to ensure stock availability.
- **Receiving and Inspection**: Manage receiving and inspection of goods from suppliers.
- **Purchasing Reports**: Access detailed reports on purchasing activities and supplier performance.

## Diagram
``` mermaid
classDiagram
    class Client {
        +int Id
        +string Name
        +string Address
        +string Phone
        +string Email
    }

    class SalesInvoice {
        +int Id
        +int ClientId
        +DateTime Date
        +decimal TotalAmount
        +Client Client
        +ICollection~DeliveryNote~ DeliveryNotes
        +Payment Payment
    }

    class DeliveryNote {
        +int Num
        +DateTime Date
        +decimal TotalExcludingVat
        +decimal NetToPay
        +TimeOnly DeliveryTime
        +int? SalesInvoiceId
        +Client? Client
        +ICollection~LigneBl~ Lines
        +SalesInvoice? SalesInvoiceNavigation
        +Transaction? Transaction
    }

    class LigneBl {
        +int Id
        +int DeliveryNoteNum
        +int ProductId
        +int Quantity
        +decimal UnitPrice
        +decimal TotalLine
        +DeliveryNote DeliveryNote
        +Product Product
    }

    class Supplier {
        +int Id
        +string Name
        +string Address
        +string Phone
        +string Email
    }

    class PurchaseOrder {
        +int Id
        +int SupplierId
        +DateTime Date
        +decimal TotalAmount
        +Supplier Supplier
        +ICollection~PurchaseOrderLine~ PurchaseOrderLines
        +Payment Payment
    }

    class PurchaseOrderLine {
        +int Id
        +int PurchaseOrderId
        +int ProductId
        +string ProductCode
        +string Description
        +int Quantity
        +decimal UnitPrice
        +decimal TotalLine
        +PurchaseOrder PurchaseOrder
        +Product Product
    }

    class Product {
        +int Id
        +string ProductCode
        +string Name
        +decimal Price
        +int StockQuantity
        +int CategoryId
        +Category Category
        +ICollection~Inventory~ Inventories
    }

    class Category {
        +int Id
        +string Name
        +string Description
        +ICollection~Product~ Products
    }

    class Inventory {
        +int Id
        +int ProductId
        +int WarehouseId
        +int Quantity
        +DateTime LastUpdated
        +Product Product
        +Warehouse Warehouse
    }

    class Warehouse {
        +int Id
        +string Name
        +string Location
        +ICollection~Inventory~ Inventories
    }

    class Payment {
        +int Id
        +int? SalesInvoiceId
        +int? PurchaseOrderId
        +decimal Amount
        +DateTime PaymentDate
        +string PaymentMethod
        +SalesInvoice SalesInvoice
        +PurchaseOrder PurchaseOrder
    }

    class Employee {
        +int Id
        +string FirstName
        +string LastName
        +string Role
        +string Email
    }

    Client "1" --> "0..*" SalesInvoice : Has
    SalesInvoice "1" --> "0..*" DeliveryNote : References
    SalesInvoice "1" --> "0..1" Payment : Paid By
    DeliveryNote "1" --> "1..*" LigneBl : Contains
    Supplier "1" --> "0..*" PurchaseOrder : Supplies
    PurchaseOrder "1" --> "1..*" PurchaseOrderLine : Contains
    PurchaseOrder "1" --> "0..1" Payment : Paid By
    PurchaseOrderLine "1" --> "1" Product : References
    LigneBl "1" --> "1" Product : References
    Product "1" --> "1" Category : Belongs To
    Product "1" --> "0..*" Inventory : Tracked In
    Inventory "1" --> "1" Warehouse : Stored In
    Category "1" --> "0..*" Product : Categorizes
    Warehouse "1" --> "0..*" Inventory : Holds
```


## Technology Stack

- **Backend**: .NET 8
- **Frontend**: Blazor Server
- **Database**: SQL Server
- **Authentication**: ASP.NET Identity
- **Logging**: Serilog with Grafana and Grafana Loki
- **Monitoring**: Grafana

## Installation

1. **Clone the repository**:
    ```sh
    git clone https://github.com/yourusername/SilkRoadErp.git
    cd SilkRoadErp
    ```

2. **Set up the database**:
    - Update the connection string in `appsettings.json`.
    - Run the migrations to set up the database schema:
      ```sh
      dotnet ef database update
      ```

3. **Run the application**:
    ```sh
    dotnet run
    ```

4. **Access the application**:
    - Open your browser and navigate to `https://localhost:5001`.

## Contributing

We welcome contributions from the community! To contribute:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Commit your changes.
4. Push to the branch.
5. Open a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For questions or support, please contact us at [nieze.benmansour@outlook.com](mailto:support@silkroaderp.com).

---

Thank you for using SilkRoadErp!
