using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOSPITAL2_LAB1.Data;
using Microsoft.AspNetCore.Authorization;
using HOSPITAL2_LAB1.Model;
using System.Security.Claims;

namespace HOSPITAL2_LAB1.Controllers
{
    [Authorize(Roles = "Receptionist")]
    public class ReceptionistsController : Controller
    {
        private readonly HOSPITAL2Context _context;

        public ReceptionistsController(HOSPITAL2Context context)
        {
            _context = context;
        }

        // GET: Receptionists
        public async Task<IActionResult> Index()
        {
            var hOSPITAL2Context = _context.Receptionists.Include(r => r.User);
            return View(await hOSPITAL2Context.ToListAsync());
        }

        // GET: Receptionists/Details/5
        public async Task<IActionResult> Details()
        {
            // Get the currently logged-in user's ID
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the doctor's information from the database
            var receptionist = await _context.Receptionists
                .FirstOrDefaultAsync(m => m.UserId == loggedInUserId);

            if (receptionist == null)
            {
                return View("WaitingForApproval");
            }

            // Pass the doctor's information to the view
            return View(receptionist);
        }


        // GET: Receptionists/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Receptionists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReceptionistId,Name,Surname,Email,UserId")] Receptionist receptionist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(receptionist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", receptionist.UserId);
            return View(receptionist);
        }

        // GET: Receptionists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Receptionists == null)
            {
                return NotFound();
            }

            var receptionist = await _context.Receptionists.FindAsync(id);
            if (receptionist == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", receptionist.UserId);
            return View(receptionist);
        }

        // POST: Receptionists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReceptionistId,Name,Surname,Email,UserId")] Receptionist receptionist)
        {
            if (id != receptionist.ReceptionistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receptionist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceptionistExists(receptionist.ReceptionistId))
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
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", receptionist.UserId);
            return View(receptionist);
        }

        // GET: Receptionists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Receptionists == null)
            {
                return NotFound();
            }

            var receptionist = await _context.Receptionists
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReceptionistId == id);
            if (receptionist == null)
            {
                return NotFound();
            }

            return View(receptionist);
        }

        // POST: Receptionists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Receptionists == null)
            {
                return Problem("Entity set 'HOSPITAL2Context.Receptionists'  is null.");
            }
            var receptionist = await _context.Receptionists.FindAsync(id);
            if (receptionist != null)
            {
                _context.Receptionists.Remove(receptionist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReceptionistExists(int id)
        {
          return (_context.Receptionists?.Any(e => e.ReceptionistId == id)).GetValueOrDefault();
        }
    }
}
