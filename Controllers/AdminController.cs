using Microsoft.AspNetCore.Mvc;
using BillingAndInvoiceSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace BillingAndInvoiceSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, string billerName, DateTime? fromDate, DateTime? toDate)
        {
            //  Session check
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            // Get invoices
            var invoices = _context.Invoices
                .OrderByDescending(i => i.Date)
                .AsQueryable();

            //  Search (Customer / Invoice No)
            if (!string.IsNullOrEmpty(search))
            {
                invoices = invoices.Where(i =>
                    i.CustomerName.Contains(search) ||
                    i.InvoiceNumber.Contains(search));
            }

            // Filter by staff/admin
            if (!string.IsNullOrEmpty(billerName))
            {
                invoices = invoices.Where(i => i.BillerName == billerName);
            }

            //  Date filter
            if (fromDate.HasValue)
            {
                invoices = invoices.Where(i => i.Date >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                invoices = invoices.Where(i => i.Date <= toDate.Value);
            }

            // Take latest 25
            var result = invoices.Take(25).ToList();

            // Summary
            ViewBag.TotalInvoices = invoices.Count();
            ViewBag.TotalRevenue = invoices.Any() ? invoices.Sum(i => i.FinalAmount) : 0;

            //  Dynamic dropdown users
            var users = _context.Users
                .Select(u => u.Name)
                .Distinct()
                .ToList();

            ViewBag.Users = users;

            return View(result);
        }
    }
}