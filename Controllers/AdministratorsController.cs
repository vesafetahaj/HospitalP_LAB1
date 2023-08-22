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
using System.Globalization;

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
        // Per personal info
        private bool HasProvidedPersonalInfo(string userId)
        {
            return _context.Administrators.Any(info => info.UserId == userId);
        }

        //Patients
        public async Task<IActionResult> Patients()
        {
            var patients = await _context.Patients.ToListAsync(); // Fetch all patients from the database
            return View(patients);
        }
        public async Task<IActionResult> SearchPatients(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // If the search string is empty or null, return all doctors
                var allPatients = await _context.Patients.Include(a => a.User).ToListAsync();
                return View("Patients", allPatients);
            }

            // Search for doctors whose name or specialization contains the search query
            var patients = await _context.Patients
                .Where(d => d.Name.Contains(query) || d.Surname.Contains(query))
                .Include(a => a.User)
                .ToListAsync();


            return View("Patients", patients);
        }

        //Doctors
        public async Task<IActionResult> Doctors()
        {
            var hOSPITAL2Context = _context.Doctors.Include(a => a.User);
            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");

            return View(await hOSPITAL2Context.ToListAsync());
        }
        public async Task<IActionResult> EditDoctor(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");
            return View(doctor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor(int id, [Bind("DoctorId,Name,Surname,Education,Specialization,Email,PhotoUrl")] Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the selected email from the doctor object
                    string selectedEmail = doctor.Email;

                    // Find the user with the selected email in the AspNetUsers table
                    var user = await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Email == selectedEmail);

                    if (user != null)
                    {
                        // Set the UserId property of the doctor entity
                        doctor.UserId = user.Id;

                        _context.Update(doctor);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Selected user not found.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.DoctorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Doctors));
            }

            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");
            return View(doctor);
        }


        private bool DoctorExists(int id)
        {
            return (_context.Doctors?.Any(e => e.DoctorId == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> DeleteDoctor(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");
            return View(doctor);

        }

        [HttpPost, ActionName("DeleteDoctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedDoctor(int id)
        {
            if (_context.Doctors == null)
            {
                return Problem("Entity set 'HOSPITAL2Context.Doctors'  is null.");
            }
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
            }

            await _context.SaveChangesAsync();
            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");

            return RedirectToAction(nameof(Doctors));
        }
        public IActionResult CreateDoctor()
        {
            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");
            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor([Bind("DoctorId,Name,Surname,Education,Specialization,Email,PhotoUrl")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                // Get the selected email from the doctor object
                string selectedEmail = doctor.Email;

                // Find the user with the selected email in the AspNetUsers table
                var user = await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Email == selectedEmail);

                if (user != null)
                {
                    // Set the UserId property of the doctor entity
                    doctor.UserId = user.Id;

                    // Add and save the doctor entity
                    _context.Add(doctor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Doctors));
                }
                else
                {
                    ModelState.AddModelError("", "Selected user not found.");
                }
            }

            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");
            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
            return View(doctor);
        }
        public async Task<IActionResult> SearchDoctors(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // If the search string is empty or null, return all doctors
                var allDoctors = await _context.Doctors.Include(a => a.User).ToListAsync();
                return View("Doctors", allDoctors);
            }

            // Search for doctors whose name or specialization contains the search query
            var doctors = await _context.Doctors
                .Where(d => d.Name.Contains(query) || d.Surname.Contains(query))
                .Include(a => a.User)
                .ToListAsync();

            // Populate the ViewBag.Name for the Specializations dropdown
            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");

            // Populate the ViewBag.Emails for the Emails dropdown
            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");

            return View("Doctors", doctors);
        }

        //Services
        public async Task<IActionResult> Services()
        {
            var services = await _context.Specializations.ToListAsync();

            return View(services);
        }

        public IActionResult CreateService()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService([Bind("SpecializationId,Name,Description,PhotoUrl")] Specialization service)
        {
            if (ModelState.IsValid)
            {
                // Get the administrator's ID from the ClaimsPrincipal
                string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Find the administrator with the logged-in user's ID
                var administrator = await _context.Administrators
                    .FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

                if (administrator != null)
                {
                    // Associate the administrator with the service
                    service.Administrator = administrator.AdminId;

                    _context.Add(service);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Services));
                }
                else
                {
                    ModelState.AddModelError("", "Administrator not found.");
                }
            }

            return View(service);
        }


        public async Task<IActionResult> EditService(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Specializations.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(int id, [Bind("SpecializationId,Name,Description,PhotoUrl,Administrator")] Specialization service)
        {
            if (id != service.SpecializationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing specialization from the DbContext
                    var existingService = await _context.Specializations.FindAsync(id);

                    // Update the properties of the existing specialization with the values from the binding model
                    existingService.Name = service.Name;
                    existingService.Description = service.Description;
                    existingService.PhotoUrl = service.PhotoUrl;
                    // Note: You might not need to update the Administrator property if you don't want to change it here

                    // Save the changes to the DbContext
                    _context.Update(existingService);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Services));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecializationExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(service);
        }
        private bool SpecializationExists(int id)
        {
            return (_context.Specializations?.Any(e => e.SpecializationId == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> DeleteService(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Specializations.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost, ActionName("DeleteService")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedService(int id)
        {
            var service = await _context.Specializations.FindAsync(id);
            if (service != null)
            {
                _context.Specializations.Remove(service);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Services));
        }

        //Appointments
        public async Task<IActionResult> Appointments(string filterDate, int? filterDoctor, int? filterPatient, int? filterService)
        {
            var appointmentsQuery = _context.Reservations
                .Include(r => r.PatientNavigation)
                .Include(r => r.DoctorNavigation)
                .Include(r => r.ServiceNavigation)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filterDate))
            {
                DateTime selectedDate = DateTime.ParseExact(filterDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                appointmentsQuery = appointmentsQuery.Where(a => a.ReservationDate.HasValue && a.ReservationDate.Value.Date == selectedDate.Date);
            }

            if (filterDoctor.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.Doctor == filterDoctor.Value);
            }

            if (filterPatient.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.Patient == filterPatient.Value);
            }

            if (filterService.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.Service == filterService.Value);
            }

            var appointments = await appointmentsQuery.ToListAsync();

            ViewBag.OldestAppointment = appointments.OrderBy(a => a.ReservationDate).FirstOrDefault();
            ViewBag.EarliestAppointment = appointments.OrderBy(a => a.ReservationDate).LastOrDefault();

            // Populate ViewBag.Doctors, ViewBag.Patients, and ViewBag.Specializations
            var doctors = await _context.Doctors.ToListAsync();
            var patients = await _context.Patients.ToListAsync();
            var services = await _context.Specializations.ToListAsync();

            ViewBag.Doctors = doctors;
            ViewBag.Patients = patients;
            ViewBag.Specializations = services;

            return View(appointments);
        }


        //Complaints

        public async Task<IActionResult> Complaints()
        {
            var complaints = await _context.Complaints.Include(r => r.PatientNavigation).ToListAsync();
            return View(complaints);
        }


    }
}
