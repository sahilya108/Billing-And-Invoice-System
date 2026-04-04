using Microsoft.AspNetCore.Mvc;

namespace BillingAndInvoiceSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            return View();
        }
    }
}