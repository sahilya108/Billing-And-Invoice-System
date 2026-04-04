using System.Linq;
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

        // Login Authontication
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (user == null)
            {
                return View();
            }

            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == user.Email);

            if (existingUser == null)
            {
                ViewBag.Error = "Email not found";
                return View(user);
            }

            if (existingUser.Password != user.Password)
            {
                ViewBag.Error = "Incorrect password";
                return View(user);
            }

            // Success (no need to check again)
            HttpContext.Session.SetString("UserEmail", existingUser.Email);
            HttpContext.Session.SetString("UserRole",existingUser.Role);
            return RedirectToAction("Index", "Home");
        }

        // Logout 
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}