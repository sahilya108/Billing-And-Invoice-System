using System.ComponentModel.DataAnnotations;

namespace BillingAndInvoiceSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }
    }
}