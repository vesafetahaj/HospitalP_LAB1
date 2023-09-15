using Microsoft.AspNetCore.Mvc;

namespace HOSPITAL2_LAB1.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Message()
        {
            return View();
        }
    }
}   
