using BillingAndInvoiceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;
using BillingAndInvoiceSystem.Helpers;
using System.Collections.Generic;

namespace BillingAndInvoiceSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        //  Admin Role Check Method
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        //  Common Role Check Method
        private bool IsAdminOrStaff()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin" || role == "Staff";
        }

        //  Product List
        public IActionResult Index()
        {
            if (!IsAdminOrStaff())
            {
                return RedirectToAction("Login", "User");
            }

            var products = _context.Products.ToList();
            return View(products);
        }

        //  Create (GET)
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
            }

            return View();
        }

        //  Create (POST)
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(product);
        }

        //  Edit (GET)
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
            }

            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //  Edit (POST)
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(product);
        }

        //  Delete
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
            }

            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Add to cart
        public IActionResult AddToCart(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin" && role != "Staff")
            {
                return RedirectToAction("Login", "User");
            }

            var product = _context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            // Get cart from session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Check if product already in cart
            var existingItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }
            // Save back to session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index");
        }
    }
}