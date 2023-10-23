using Microsoft.AspNetCore.Mvc;

namespace ThomasianMemoir.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Homepage()
        {
            return View();
        }

        public IActionResult Freshmen()
        {
            return View();
        }

        public IActionResult Sophomore()
        {
            return View();
        }

        public IActionResult Senior()
        {
            return View();
        }

        public IActionResult Junior()
        {
            return View();
        }
    }
}
