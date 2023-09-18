using HOSPITAL2_LAB1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HOSPITAL2_LAB1.Controllers
{
    public class SherbimetController : Controller
    {
        private readonly HOSPITAL2Context _context;

        public SherbimetController(HOSPITAL2Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Sherbimet()
        {
            var services = await _context.Specializations.ToListAsync();

            return View(services);
        }
    }
}
