using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOSPITAL2_LAB1.Data;
using HOSPITAL2_LAB1.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HOSPITAL2_LAB1.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdministratorsController : Controller
    {
        private readonly HOSPITAL2Context _context;

        public AdministratorsController(HOSPITAL2Context context)
        {
            _context = context;
        }

        // GET: Administrators
        public async Task<IActionResult> Index()
        {
            var hOSPITAL2Context = _context.Administrators.Include(a => a.User);
            ViewBag.HasProvidedPersonalInfo = HasProvidedPersonalInfo(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString().ToLower());
            return View(await hOSPITAL2Context.ToListAsync());
        }
        public async Task<IActionResult> Details()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var administrator = await _context.Administrators
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.UserId == loggedInUserId);

            return administrator != null ? View(administrator) : (IActionResult)NotFound();
        }

        /*
        // GET: Administrators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Administrators == null)
            {
                return NotFound();
            }

            var administrator = await _context.Administrators
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (administrator == null)
            {
                return NotFound();
            }

            return View(administrator);
        }*/

        // GET: Administrators/Create
        public IActionResult PersonalInfo()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (HasProvidedPersonalInfo(loggedInUserId))
            {
                return RedirectToAction(nameof(Details));
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Administrators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonalInfo([Bind("AdminId,Name,Surname")] Administrator administrator)
        {
            if (ModelState.IsValid)
            {
                // Get the user's ID from the ClaimsPrincipal
                string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Set the UserId property of the administrator object
                administrator.UserId = loggedInUserId;

                _context.Add(administrator);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = administrator.AdminId }); // Redirect to the Details action with the created Administrator's ID
            }

            return View(administrator);
        }

        // GET: Administrators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Administrators == null)
            {
                return NotFound();
            }

            var administrator = await _context.Administrators.FindAsync(id);
            if (administrator == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", administrator.UserId);
            return View(administrator);
        }

        // POST: Administrators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,Name,Surname,UserId")] Administrator administrator)
        {
            if (id != administrator.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(administrator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministratorExists(administrator.AdminId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", administrator.UserId);
            return View(administrator);
        }

        // GET: Administrators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Administrators == null)
            {
                return NotFound();
            }

            var administrator = await _context.Administrators
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (administrator == null)
            {
                return NotFound();
            }

            return View(administrator);
        }

        // POST: Administrators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Administrators == null)
            {
                return Problem("Entity set 'HOSPITAL2Context.Administrators'  is null.");
            }
            var administrator = await _context.Administrators.FindAsync(id);
            if (administrator != null)
            {
                _context.Administrators.Remove(administrator);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministratorExists(int id)
        {
          return (_context.Administrators?.Any(e => e.AdminId == id)).GetValueOrDefault();
        }
        // In your controller or service
        private bool HasProvidedPersonalInfo(string userId)
        {
            return _context.Administrators.Any(info => info.UserId == userId);
        }
        public async Task<IActionResult> Patients()
        {
            var patients = await _context.Patients.ToListAsync(); // Fetch all patients from the database
            return View(patients);
        }


    }
}
