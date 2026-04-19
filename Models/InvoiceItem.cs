using System.ComponentModel.DataAnnotations.Schema;

namespace BillingAndInvoiceSystem.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public int InvoiceId { get; set; }

        [NotMapped]
        public decimal Total => Price * Quantity;
    }
}