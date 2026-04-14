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

                return RedirectToAction("Index");
            }

            return View(customer);
        }

        //  Search Customer
        public IActionResult Search(string searchTerm)
        {
            var customers = _context.Customers
                .Where(c => c.Phone.Contains(phone))
                .ToList();

            return View("Index", customers);
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