using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes.Request
{
    public class GetReceipNotesQueryParams
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortProprety { get; set; }
        public string? SortOrder { get; set; }
        public string? SearchKeyword { get; set; }
        public string? ProviderId { get; set; }
        public bool? IsInvoiced { get; set; }
        public int? InvoiceId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
