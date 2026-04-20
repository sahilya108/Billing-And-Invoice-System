using BillingAndInvoiceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BillingAndInvoiceSystem.Controllers
{
    public class StaffController : Controller
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, DateTime? fromDate, DateTime? toDate)
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserName");

            if (role != "Staff")
            {
                return RedirectToAction("Login", "User");
            }

            var invoices = _context.Invoices
                .Where(i => i.BillerName == userName) //  ONLY STAFF DATA
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                invoices = invoices.Where(i =>
                    i.CustomerName.Contains(search) ||
                    i.InvoiceNumber.Contains(search));
            }

            // Date Filter
            if (fromDate.HasValue)
            {
                invoices = invoices.Where(i => i.Date >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                invoices = invoices.Where(i => i.Date <= toDate.Value);
            }

            // Summary
            ViewBag.TotalInvoices = invoices.Count();
            ViewBag.TotalRevenue = invoices.Sum(i => i.FinalAmount);

            //  Latest 25
            var list = invoices
                .OrderByDescending(i => i.Date)
                .Take(25)
                .ToList();

            return View(list);
        }
    }
}