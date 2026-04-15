using BillingAndInvoiceSystem.Helpers;
using BillingAndInvoiceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace BillingAndInvoiceSystem.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // View Cart
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart")
                       ?? new List<CartItem>();

            return View(cart);
        }

        // 🔥 Update Quantity (WITH STOCK VALIDATION)
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == productId);

                if (item != null)
                {
                    var product = _context.Products.Find(productId);

                    if (product != null)
                    {
                        // ❗ Minimum quantity = 1
                        if (quantity < 1)
                        {
                            quantity = 1;
                        }

                        // ❗ Maximum = stock
                        if (quantity > product.Stock)
                        {
                            quantity = product.Stock;
                            TempData["Error"] = $"Only {product.Stock} items available!";
                        }
                    }

                    item.Quantity = quantity;
                }

                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }

        // Remove Item
        public IActionResult Remove(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == productId);

                if (item != null)
                {
                    cart.Remove(item);
                }

                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }

        // Clear Cart
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            TempData["Success"] = "Cart cleared!";
            return RedirectToAction("Index");
        }

        // Checkout → Invoice
        public IActionResult Checkout()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (customerId == null || cart == null || !cart.Any())
            {
                TempData["Error"] = "Select customer and add products first ❌";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Generate", "Invoice");
        }
    }
}