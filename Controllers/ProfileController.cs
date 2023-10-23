using Microsoft.AspNetCore.Mvc;

namespace ThomasianMemoir.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult EditProfile()
        {
            return View();
        }
    }
}
