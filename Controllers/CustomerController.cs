using BillingAndInvoiceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BillingAndInvoiceSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // View Customers
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }

        // Add Customer (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Add Customer (POST)
        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();

                TempData["Success"] = "Customer added successfully";

                return RedirectToAction("Index");
            }

            return View(customer);
        }

        //  Search Customer
        public IActionResult Search(string searchTerm)
        {
            var customers = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                customers = customers.Where(c =>
                    c.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    c.Phone.Contains(searchTerm));
            }

            var result = customers.ToList();

            if (!result.Any())
            {
                ViewBag.Message = "No customer found";
            }

            return View("Index", result);
        }

        // SELECT CUSTOMER 
        public IActionResult Select(int id)
        {
            var customer = _context.Customers.Find(id);

            if (customer != null)
            {
                HttpContext.Session.SetInt32("CustomerId", customer.Id);
                HttpContext.Session.SetString("CustomerName", customer.Name);
            }

            return RedirectToAction("Index", "Billing");
        }
    }
}