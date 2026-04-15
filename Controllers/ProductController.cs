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

        // Admin Role Check
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        // Admin or Staff Check
        private bool IsAdminOrStaff()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin" || role == "Staff";
        }

        // Product List
        public IActionResult Index()
        {
            if (!IsAdminOrStaff())
            {
                return RedirectToAction("Login", "User");
            }

            var products = _context.Products.ToList();
            return View(products);
        }

        // Create (GET)
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
            }

            return View();
        }

        // Create (POST)
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

                TempData["Success"] = "Product added successfully!";
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // Edit (GET)
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

        // Edit (POST)
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

                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // Delete
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

            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction("Index");
        }

        // ?? ADD TO CART (WITH STOCK VALIDATION)
        public IActionResult AddToCart(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Staff") // ? Only staff should add to cart
            {
                return RedirectToAction("Login", "User");
            }

            var product = _context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart")
                       ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (existingItem != null)
            {
                //  CHECK STOCK LIMIT
                if (existingItem.Quantity < product.Stock)
                {
                    existingItem.Quantity++;
                    TempData["Success"] = "Product quantity updated!";
                }
                else
                {
                    TempData["Error"] = $"Only {product.Stock} items available!";
                }
            }
            else
            {
                if (product.Stock > 0)
                {
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        Quantity = 1
                    });

                    TempData["Success"] = "Product added to cart!";
                }
                else
                {
                    TempData["Error"] = "Product out of stock!";
                }
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index");
        }

        //  SEARCH PRODUCTS
        public IActionResult Search(string searchTerm)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            var result = products.ToList();

            if (!result.Any())
            {
                ViewBag.Message = "No product found ?";
            }

            return View("Index", result);
        }
    }
}