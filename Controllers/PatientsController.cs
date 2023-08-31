using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOSPITAL2_LAB1.Data;
using HOSPITAL2_LAB1.Model;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HOSPITAL2_LAB1.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientsController : Controller
    {
        private readonly HOSPITAL2Context _context;

        public PatientsController(HOSPITAL2Context context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var hOSPITAL2Context = _context.Patients.Include(p => p.User);
            return View(await hOSPITAL2Context.ToListAsync());
        }
        public async Task<IActionResult> Details()
        {
            // Get the currently logged-in user's ID
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the doctor's information from the database
            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.UserId == loggedInUserId);

            if (patient == null)
            {
                return View("WaitingForApproval");
            }

            // Pass the doctor's information to the view
            return View(patient);
        }

        /*

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }
        */
        // GET: Patients/Create
        public IActionResult Create()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (HasProvidedPersonalInfo(loggedInUserId))
            {
                return RedirectToAction(nameof(Details));
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,Name,Surname,Gender,Birthday,Address,Phone")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                // Get the user's ID from the ClaimsPrincipal
                string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Set the UserId property of the administrator object
                patient.UserId = loggedInUserId;

                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", patient.UserId);
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", patient.UserId);
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,Name,Surname,Gender,Birthday,Address,Phone,UserId")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
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
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", patient.UserId);
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Patients == null)
            {
                return Problem("Entity set 'HOSPITAL2Context.Patients'  is null.");
            }
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
          return (_context.Patients?.Any(e => e.PatientId == id)).GetValueOrDefault();
        }
        private bool HasProvidedPersonalInfo(string userId)
        {
            return _context.Patients.Any(info => info.UserId == userId);
        }

        public IActionResult CreateAppointment()
        {
            ViewBag.DoctorList = new SelectList(_context.Doctors, "DoctorId", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment([Bind("ReservationID,ReservationDate,ReservationTime,Doctor")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
              
                string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var patient = await _context.Patients.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

                if (patient != null)
                {
                    reservation.Patient = patient.PatientId;

                    _context.Add(reservation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Appointments));

                }
                else
                {
                    ModelState.AddModelError("", "You have to provide personal info first.");
                }
            }
            ViewBag.DoctorList = new SelectList(_context.Doctors, "DoctorId", "FullName");
            return View(reservation);
        }
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.Reservations.Include(r => r.DoctorNavigation).ToListAsync(); 
            return View(appointments);
        }


        //edit

        public async Task<IActionResult> EditAppointment(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewBag.DoctorList = new SelectList(_context.Doctors, "DoctorId", "FullName");

            return View(reservation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAppointment(int id, [Bind("ReservationId,ReservationDate,ReservationTime,Doctor")] Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(reservation.ReservationId))
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

            // Repopulate the dropdown list for doctors
            ViewBag.DoctorList = new SelectList(_context.Doctors, "DoctorId", "FullName");

            return View(reservation);
        }


        private bool AppointmentExists(int id)
        {
            return (_context.Reservations?.Any(e => e.ReservationId == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> DeleteAppointment(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
              
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            ViewBag.DoctorList = new SelectList(_context.Doctors, "DoctorId", "FullName");
            return View(reservation);

        }

        [HttpPost, ActionName("DeleteAppointment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAppointment(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'HOSPITAL2Context.Reservations'  is null.");
            }
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();

            ViewBag.DoctorList = new SelectList(_context.Doctors, "DoctorId", "FullName");

            return RedirectToAction(nameof(Appointments));
        }
    }
}
