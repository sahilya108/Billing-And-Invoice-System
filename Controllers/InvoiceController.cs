using BillingAndInvoiceSystem.Helpers;
using BillingAndInvoiceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingAndInvoiceSystem.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly AppDbContext _context;

        public InvoiceController(AppDbContext context)
        {
            _context = context;
        }

        // 🔥 ADD PARAMETERS HERE
        public IActionResult Generate(decimal? discountAmount, decimal? discountPercent)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            var customerName = HttpContext.Session.GetString("CustomerName");
            var billerName = HttpContext.Session.GetString("UserName") ?? "Admin";
            var invoiceNumber = "INV-" + DateTime.Now.Ticks;

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (customerId == null || cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // TOTAL CALCULATION
            var total = cart.Sum(x => x.TotalPrice);

            decimal finalAmount = total;
            decimal appliedDiscountAmount = 0;
            decimal appliedDiscountPercent = 0;

            // DISCOUNT LOGIC
            if (discountAmount.HasValue && discountAmount > 0)
            {
                appliedDiscountAmount = discountAmount.Value;
                finalAmount = total - appliedDiscountAmount;
            }
            else if (discountPercent.HasValue && discountPercent > 0)
            {
                appliedDiscountPercent = discountPercent.Value;
                appliedDiscountAmount = total * (appliedDiscountPercent / 100);
                finalAmount = total - appliedDiscountAmount;
            }

            // Create Invoice
            var invoice = new Invoice
            {
                CustomerId = customerId.Value,
                CustomerName = customerName,
                BillerName = billerName,
                InvoiceNumber = invoiceNumber,
                Date = DateTime.Now,

                TotalAmount = total,
                DiscountAmount = appliedDiscountAmount,
                DiscountPercent = appliedDiscountPercent,
                FinalAmount = finalAmount,

                Items = new List<InvoiceItem>()
            };

            foreach (var item in cart)
            {
                invoice.Items.Add(new InvoiceItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity
                });

                var product = _context.Products.Find(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                }
            }

            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Details", new { id = invoice.Id });
        }

        public IActionResult Details(int id)
        {
            var invoice = _context.Invoices
                .Where(i => i.Id == id)
                .Select(i => new Invoice
                {
                    Id = i.Id,
                    CustomerName = i.CustomerName,
                    BillerName = i.BillerName,
                    InvoiceNumber = i.InvoiceNumber,
                    Date = i.Date,
                    TotalAmount = i.TotalAmount,
                    DiscountAmount = i.DiscountAmount,
                    DiscountPercent = i.DiscountPercent,
                    FinalAmount = i.FinalAmount,
                    Items = _context.InvoiceItems
                                .Where(x => x.InvoiceId == i.Id)
                                .ToList()
                })
                .FirstOrDefault();

            return View(invoice);
        }
    }
}