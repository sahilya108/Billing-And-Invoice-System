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

        // VIEW STAFF LIST
        public IActionResult StaffList()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login");
            }

            var staff = _context.Users.Where(u => u.Role == "Staff").ToList();

            return View(staff);
        }

        // EDIT STAFF (GET)
        public IActionResult EditStaff(int id)
        {
            var staff = _context.Users.Find(id);

            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // EDIT STAFF (POST)
        [HttpPost]
        public IActionResult EditStaff(User user)
        {
            var existingUser = _context.Users.Find(user.Id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // update only fields
            existingUser.Name = user.Name;
            existingUser.Password = user.Password;

            _context.SaveChanges();

            TempData["Success"] = "Staff updated successfully";

            return RedirectToAction("StaffList");
        }

        // DELETE STAFF
        public IActionResult DeleteStaff(int id)
        {
            var staff = _context.Users.Find(id);

            if (staff == null)
            {
                return NotFound();
            }

            _context.Users.Remove(staff);
            _context.SaveChanges();

            TempData["Success"] = "Staff deleted successfully";

            return RedirectToAction("StaffList");
        }

        // CHANGE PASSWORD (GET)
        public IActionResult ChangePassword(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // CHANGE PASSWORD (POST)
        [HttpPost]
        public IActionResult ChangePassword(int id, string newPassword)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                TempData["Error"] = "Password cannot be empty";
                return RedirectToAction("ChangePassword", new { id });
            }

            user.Password = newPassword; // later we can hash
            _context.SaveChanges();

            TempData["Success"] = "Password updated successfully";

            return RedirectToAction("StaffList");
        }

        // CREATE STAFF (GET)
        public IActionResult CreateStaff()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // CREATE STAFF (POST)
        [HttpPost]
        public IActionResult CreateStaff(User user)
        {
            user.Role = "Staff";

            ModelState.Remove("Role"); 

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Staff created successfully";

            return RedirectToAction("StaffList");
        }
    }
}