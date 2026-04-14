using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace BillingAndInvoiceSystem.Controllers
{
    public class BillingController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Staff" && role != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            // Get selected customer from session
            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");

            return View();
        }
    }
}