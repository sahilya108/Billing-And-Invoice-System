using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace BillingAndInvoiceSystem.Controllers
{
    public class BillingController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Staff")
            {
                return RedirectToAction("Login", "User");
            }

            // Get selected customer from session
            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");

            return View();
        }

        // ADMIN → SWITCH TO STAFF MODE
        public IActionResult StartBilling()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            // Switch role to Staff
            HttpContext.Session.SetString("UserRole", "Staff");

            return RedirectToAction("Index");
        }
    }
}