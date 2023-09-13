using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HOSPITAL2_LAB1.Model;
using HOSPITAL2_LAB1.Data;

namespace HOSPITAL2_LAB1.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly HOSPITAL2Context _context;

        public AboutUsController(HOSPITAL2Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> AboutUs()
        {
            var doctors = await _context.Doctors
                .Include(a => a.User)
                .Include(r => r.SpecializationNavigation)
                .ToListAsync();

            return View(doctors);
        }

    }
}
