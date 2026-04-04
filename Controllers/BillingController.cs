using Microsoft.AspNetCore.Mvc;

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

            return View();
        }
    }
}