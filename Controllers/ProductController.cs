using BillingAndInvoiceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BillingAndInvoiceSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        //  Common Role Check Method
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        //  Product List
        public IActionResult Index()
        {
            if (!IsAdmin())
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
    }
}