using BillingAndInvoiceSystem.Helpers;
using BillingAndInvoiceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BillingAndInvoiceSystem.Controllers
{
    public class CartController : Controller
    {
        // View Cart
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart")
                       ?? new List<CartItem>();

            return View(cart);
        }

        // Update Quantity
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == productId);

                if (item != null)
                {
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

        // CLEAR CART
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }

        //  CHECKOUT (GO TO INVOICE)
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