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

        public IActionResult Generate()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            var customerName = HttpContext.Session.GetString("CustomerName");

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (customerId == null || cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // Create Invoice
            var invoice = new Invoice
            {
                CustomerId = customerId.Value,
                CustomerName = customerName,
                Date = DateTime.Now,
                TotalAmount = cart.Sum(x => x.TotalPrice),
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

                //  Reduce stock
                var product = _context.Products.Find(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                }
            }

            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            // Clear cart after billing
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
                    Date = i.Date,
                    TotalAmount = i.TotalAmount,
                    Items = _context.InvoiceItems.Where(x => x.InvoiceId == i.Id).ToList()
                })
                .FirstOrDefault();

            return View(invoice);
        }
    }
}