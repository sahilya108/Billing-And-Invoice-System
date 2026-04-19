using System;
using System.Collections.Generic;

namespace BillingAndInvoiceSystem.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalAmount { get; set; }

        public List<InvoiceItem> Items { get; set; }

        public string BillerName { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal FinalAmount { get; set; }
        public bool IsPdf { get; set; } = false;
    }
}