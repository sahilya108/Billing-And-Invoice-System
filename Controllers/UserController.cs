using BillingAndInvoiceSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace BillingAndInvoiceSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Register Page
        public IActionResult Register()
        {
            return View();
        }

        // POST: Save User
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Login Page (temporary)
        public IActionResult Login()
        {
            return View();
        }
    }
}