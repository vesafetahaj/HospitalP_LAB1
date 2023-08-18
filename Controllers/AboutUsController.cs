using Microsoft.AspNetCore.Mvc;

namespace HOSPITAL2_LAB1.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
