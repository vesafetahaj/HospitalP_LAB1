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

namespace HOSPITAL2_LAB1.Controllers
{
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
            return View(await hOSPITAL2Context.ToListAsync());
        }

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
        }

        // GET: Administrators/Create
        public IActionResult PersonalInfo()
        {
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
                return RedirectToAction(nameof(Index));
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
    }
}
