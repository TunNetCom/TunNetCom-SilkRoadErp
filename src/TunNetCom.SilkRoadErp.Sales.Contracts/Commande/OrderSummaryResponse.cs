using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Commande;
public class OrderSummaryResponse
{
    public int OrderNumber { get; set; }
    public int? SupplierId { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalExcludingVat { get; set; }
    public decimal TotalVat { get; set; }
    public decimal NetToPay { get; set; }
}

