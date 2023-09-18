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
using System.Numerics;

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

        public async Task<IActionResult> Index()
        {
            int numberOfPatients = _context.Patients.Count();
            int numberOfDoctors = _context.Doctors.Count();
           int numberOfAppointments = _context.Reservations.Count();   

            ViewData["NumberOfPatients"] = numberOfPatients;
            ViewData["NumberOfDoctors"] = numberOfDoctors;
            ViewData["NumberOfAppointments"] = numberOfAppointments;
            return View();
        }
        public async Task<IActionResult> Details()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var administrator = await _context.Administrators
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.UserId == loggedInUserId);

            return administrator != null ? View(administrator) : (IActionResult)NotFound();
        }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonalInfo([Bind("AdminId,Name,Surname")] Administrator administrator)
        {
            if (ModelState.IsValid)
            {
                string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                administrator.UserId = loggedInUserId;

                _context.Add(administrator);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = administrator.AdminId }); // Redirect to the Details action with the created Administrator's ID
            }

            return View(administrator);
        }
        public IActionResult EditPersonalInfo()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Administrator administrator = _context.Administrators.FirstOrDefault(a => a.UserId == loggedInUserId);

            if (administrator == null)
            {
                return NotFound();
            }

            return View(administrator);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPersonalInfo(int id, [Bind("AdminId,Name,Surname")] Administrator updatedAdministrator)
        {
            if (id != updatedAdministrator.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    Administrator existingAdministrator = await _context.Administrators.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

                    if (existingAdministrator == null)
                    {
                        return NotFound();
                    }

                    existingAdministrator.Name = updatedAdministrator.Name;
                    existingAdministrator.Surname = updatedAdministrator.Surname;

                    _context.Update(existingAdministrator);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = existingAdministrator.AdminId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return RedirectToAction("Details", new { id = id });
                }
            }

            return View(updatedAdministrator);
        }

        /*
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
        }*/

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
        public async Task<IActionResult> Patients(string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "Date" ? "date_desc" : "Date";

            var patientsQuery = _context.Patients.AsQueryable();

            switch (sortOrder)
            {
                case "name_desc":
                    patientsQuery = patientsQuery.OrderByDescending(p => p.Name);
                    break;
                case "name_asc": 
                    patientsQuery = patientsQuery.OrderBy(p => p.Name);
                    break;
                case "Date":
                    patientsQuery = patientsQuery.OrderBy(p => p.Birthday);
                    break;
                case "date_desc":
                    patientsQuery = patientsQuery.OrderByDescending(p => p.Birthday);
                    break;
                default:
                    ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : sortOrder; 
                    break;
            }

            var patients = await patientsQuery.Include(a => a.User).ToListAsync();
            return View(patients);
        }

        public async Task<IActionResult> SearchPatients(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                var allPatients = await _context.Patients.Include(a => a.User).ToListAsync();
                return View("Patients", allPatients);
            }

            var patients = await _context.Patients
                .Where(d => (d.Name + " " + d.Surname).Contains(query))
                .Include(a => a.User)
                .ToListAsync();

            return View("Patients", patients);
        }


        //Doctors
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _context.Doctors
                .Include(a => a.User)
                .Include(r => r.SpecializationNavigation)
                .ToListAsync();

            return View(doctors);
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
            var doctorEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@doctor.com")).Select(u => u.Email).ToList();
            SelectList doctorEmailsSelectList = new SelectList(doctorEmails);
            ViewData["Emails"] = doctorEmailsSelectList; 
            return View(doctor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor(int id, [Bind("DoctorId,Name,Surname,Education,Specialization,Email,PhotoUrl")] Doctor doctor)
        {
            if (string.IsNullOrWhiteSpace(doctor.Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
            }
            if (string.IsNullOrWhiteSpace(doctor.Surname))
            {
                ModelState.AddModelError("Surname", "Surname is required.");
            }
            if (string.IsNullOrWhiteSpace(doctor.Education))
            {
                ModelState.AddModelError("Education", "Education is required.");
            }

            if (string.IsNullOrWhiteSpace(doctor.Email))
            {
                ModelState.AddModelError("Email", "Email is required.");
            }

            if (doctor.Specialization == null)
            {
                ModelState.AddModelError("Specialization", "Specialization is required.");
            }
            if (string.IsNullOrWhiteSpace(doctor.PhotoUrl))
            {
                ModelState.AddModelError("PhotoUrl", "Photo URL is required.");
            }
            var doctorEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@doctor.com")).Select(u => u.Email).ToList();
            SelectList doctorEmailsSelectList = new SelectList(doctorEmails);
            if (ModelState.IsValid)
            {
                try
                {
                    string selectedEmail = doctor.Email;

                    var user = await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Email == selectedEmail);
                    

                    if (user != null)
                    {
                        var existingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
                      
                        if (existingDoctor != null && existingDoctor.DoctorId != doctor.DoctorId)
                        {
                            ModelState.AddModelError("Email", "A doctor with the same email already exists.");
                            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
                           
                            ViewData["Emails"] = doctorEmailsSelectList;
                            return View(doctor);
                        }

                        doctor.UserId = user.Id;
                        existingDoctor.Name = doctor.Name;
                        existingDoctor.Surname = doctor.Surname;
                        existingDoctor.Education = doctor.Education;
                        existingDoctor.Specialization = doctor.Specialization;
                        existingDoctor.Email = doctor.Email;
                        existingDoctor.PhotoUrl = doctor.PhotoUrl;
                        existingDoctor.UserId = user.Id;

                        await _context.SaveChangesAsync();


                        return RedirectToAction(nameof(Doctors));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Selected user not found.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Concurrency issue occurred.");
                }
            }

            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
           
            ViewData["Emails"] = doctorEmailsSelectList;

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
            var doctorEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@doctor.com")).Select(u => u.Email).ToList();
            SelectList doctorEmailsSelectList = new SelectList(doctorEmails);
            ViewData["Emails"] = doctorEmailsSelectList;
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
            var doctorEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@doctor.com")).Select(u => u.Email).ToList();
            SelectList doctorEmailsSelectList = new SelectList(doctorEmails);
            ViewData["Emails"] = doctorEmailsSelectList;
            return RedirectToAction(nameof(Doctors));
        }
        public IActionResult CreateDoctor()
        {
            var doctorEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@doctor.com")).Select(u => u.Email).ToList();
            SelectList doctorEmailsSelectList = new SelectList(doctorEmails);
            ViewData["Emails"] = doctorEmailsSelectList;

            var specializations = _context.Specializations.ToList();
            ViewData["Name"] = new SelectList(specializations, "SpecializationId", "Name");


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor([Bind("DoctorId,Name,Surname,Education,Specialization,Email,PhotoUrl")] Doctor doctor)
        {
            if (string.IsNullOrWhiteSpace(doctor.Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
            }
            if (string.IsNullOrWhiteSpace(doctor.Surname))
            {
                ModelState.AddModelError("Surname", "Surname is required.");
            }
            if (string.IsNullOrWhiteSpace(doctor.Education))
            {
                ModelState.AddModelError("Education", "Education is required.");
            }

            if (string.IsNullOrWhiteSpace(doctor.Email))
            {
                ModelState.AddModelError("Email", "Email is required.");
            }

            if (doctor.Specialization == null)
            {
                ModelState.AddModelError("Specialization", "Specialization is required.");
            }
            if (string.IsNullOrWhiteSpace(doctor.PhotoUrl))
            {
                ModelState.AddModelError("PhotoUrl", "Photo URL is required.");
            }
            var doctorEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@doctor.com")).Select(u => u.Email).ToList();
            SelectList doctorEmailsSelectList = new SelectList(doctorEmails);
            if (ModelState.IsValid)
            {
                if (_context.Doctors.Any(d => d.Email == doctor.Email))
                {
                    ModelState.AddModelError("Email", "This doctor already exists.");
                    ViewData["Emails"] = doctorEmailsSelectList;
                    ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
                    return View(doctor);
                }

                string selectedEmail = doctor.Email;

                var user = await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Email == selectedEmail);

                if (user != null)
                {
                    doctor.UserId = user.Id;

                    _context.Add(doctor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Doctors));

                }
                else
                {
                    ModelState.AddModelError("", "Selected user not found.");
                }
            }

            ViewData["Emails"] = doctorEmailsSelectList;
            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
            return View(doctor);
        }

        public async Task<IActionResult> SearchDoctors(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                var allDoctors = await _context.Doctors.Include(a => a.User).ToListAsync();
                return View("Doctors", allDoctors);
            }

            var doctors = await _context.Doctors
                .Where(d => d.Name.Contains(query) || d.Surname.Contains(query))
                .Include(a => a.User).Include(b=> b.SpecializationNavigation)
                .ToListAsync();

            ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");

            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");

            return View("Doctors", doctors);
        }

        //Services
        public async Task<IActionResult> Services()
        {
            var services = await _context.Specializations.Include(a=>a.AdministratorNavigation).ToListAsync();

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
            if (string.IsNullOrWhiteSpace(service.Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
            }

            if (string.IsNullOrWhiteSpace(service.Description))
            {
                ModelState.AddModelError("Description", "Description is required.");
            }

            if (string.IsNullOrWhiteSpace(service.PhotoUrl))
            {
                ModelState.AddModelError("PhotoUrl", "Photo URL is required.");
            }

            if (ModelState.IsValid)
            {
                if (_context.Specializations.Any(s => s.Name == service.Name))
                {
                    ModelState.AddModelError("Name", "A service with the same name already exists.");
                    return View(service);
                }

                string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var administrator = await _context.Administrators.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

                if (administrator != null)
                {
                    service.Administrator = administrator.AdminId;

                    _context.Add(service);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Services));

                }
                else
                {
                    ModelState.AddModelError("", "You have to provide personal info first.");
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
            if (string.IsNullOrWhiteSpace(service.Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
            }

            if (string.IsNullOrWhiteSpace(service.Description))
            {
                ModelState.AddModelError("Description", "Description is required.");
            }

            if (string.IsNullOrWhiteSpace(service.PhotoUrl))
            {
                ModelState.AddModelError("PhotoUrl", "Photo URL is required.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var existingService = await _context.Specializations.FindAsync(id);

                    existingService.Name = service.Name;
                    existingService.Description = service.Description;
                    existingService.PhotoUrl = service.PhotoUrl;

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
        public async Task<IActionResult> Appointments(string filterDate, int? filterDoctor, int? filterPatient)
        {
            var appointmentsQuery = _context.Reservations
                .Include(r => r.PatientNavigation)
                .Include(r => r.DoctorNavigation)
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

            

            var appointments = await appointmentsQuery.ToListAsync();
            var appointmentsWithDate = appointments.Where(a => a.ReservationDate.HasValue).ToList();
            var oldestAppointment = appointmentsWithDate.OrderBy(a => a.ReservationDate).FirstOrDefault();
            var earliestAppointment = appointmentsWithDate.OrderByDescending(a => a.ReservationDate).FirstOrDefault();

            ViewBag.OldestAppointment = oldestAppointment;
            ViewBag.EarliestAppointment = earliestAppointment;


            var doctors = await _context.Doctors.ToListAsync();
            var patients = await _context.Patients.ToListAsync();

            ViewBag.Doctors = doctors;
            ViewBag.Patients = patients;

            return View(appointments);
        }


        //Complaints

        public async Task<IActionResult> Complaints()
        {
            var complaints = await _context.Complaints.Include(r => r.PatientNavigation).ToListAsync();
            return View(complaints);
        }

        //Contact Forms

        public async Task<IActionResult> ContactForms()
        {
            var contactForms = await _context.ContactForms.Include(r => r.PatientNavigation).ToListAsync();
            return View(contactForms);
        }

        //Receptionists


        public async Task<IActionResult> Receptionists()
        {
            var hOSPITAL2Context = _context.Receptionists.Include(r => r.User);
            return View(await hOSPITAL2Context.ToListAsync());
        }
        public IActionResult CreateReceptionist()
        {
            var receptionistsEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@receptionist.com")).Select(u => u.Email).ToList();
            SelectList receptionistsEmailsSelectList = new SelectList(receptionistsEmails);

            ViewData["Emails"] = receptionistsEmailsSelectList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReceptionist([Bind("ReceptionistId,Name,Surname,Email")] Receptionist receptionist)
        {
            var receptionistsEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@receptionist.com")).Select(u => u.Email).ToList();
            SelectList receptionistsEmailsSelectList = new SelectList(receptionistsEmails);

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(receptionist.Name))
                {
                    ModelState.AddModelError("Name", "Name is required.");
                }
                if (string.IsNullOrWhiteSpace(receptionist.Surname))
                {
                    ModelState.AddModelError("Surname", "Surname is required.");
                }
                if (string.IsNullOrWhiteSpace(receptionist.Email))
                {
                    ModelState.AddModelError("Email", "Email is required.");
                }
                else
                {
                    string selectedEmail = receptionist.Email;

                    var user = await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Email == selectedEmail);

                    if (user != null)
                    {
                        receptionist.UserId = user.Id;

                        if (_context.Receptionists.Any(r => r.Email == receptionist.Email))
                        {
                            ModelState.AddModelError("Email", "This receptionist already exists.");
                            ViewData["Emails"] = receptionistsEmailsSelectList;
                            return View(receptionist);
                        }

                        _context.Add(receptionist);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Receptionists));
                    }
                }
            }

            ViewData["Emails"] = receptionistsEmailsSelectList;

            return View(receptionist);
        }


        public async Task<IActionResult> EditReceptionist(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receptionist = await _context.Receptionists.FindAsync(id);
            if (receptionist == null)
            {
                return NotFound();
            }

            var receptionistsEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@receptionist.com")).Select(u => u.Email).ToList();
            ViewData["Emails"] = new SelectList(receptionistsEmails);

            return View(receptionist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReceptionist(int id, [Bind("ReceptionistId,Name,Surname,Email")] Receptionist receptionist)
        {
            if (id != receptionist.ReceptionistId)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(receptionist.Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
            }
            if (string.IsNullOrWhiteSpace(receptionist.Surname))
            {
                ModelState.AddModelError("Surname", "Surname is required.");
            }
            if (string.IsNullOrWhiteSpace(receptionist.Email))
            {
                ModelState.AddModelError("Email", "Email is required.");
            }

            var receptionistsEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@receptionist.com")).Select(u => u.Email).ToList();
            SelectList receptionistsEmailsSelectList = new SelectList(receptionistsEmails);

            if (ModelState.IsValid)
            {
                try
                {
                    string selectedEmail = receptionist.Email;

                    var user = await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Email == selectedEmail);

                    if (user != null)
                    {
                        var existingReceptionist = await _context.Receptionists.FirstOrDefaultAsync(r => r.UserId == user.Id);

                        if (existingReceptionist != null && existingReceptionist.ReceptionistId != receptionist.ReceptionistId)
                        {
                            ModelState.AddModelError("Email", "A receptionist with the same email already exists.");
                            ViewData["Emails"] = receptionistsEmailsSelectList;
                            return View(receptionist);
                        }

                        receptionist.UserId = user.Id;

                        existingReceptionist.Name = receptionist.Name;
                        existingReceptionist.Surname = receptionist.Surname;
                        existingReceptionist.Email = receptionist.Email;
                        existingReceptionist.UserId = receptionist.UserId; 

                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Receptionists));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Selected user not found.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Concurrency issue occurred.");
                }
            }

            ViewData["Emails"] = receptionistsEmailsSelectList;

            return View(receptionist);
        }

        public async Task<IActionResult> DeleteReceptionist(int? id)
        {
            if (id == null || _context.Receptionists == null)
            {
                return NotFound();
            }

            var receptionist = await _context.Receptionists
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ReceptionistId == id);
            if (receptionist == null)
            {
                return NotFound();
            }
            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");
            return View(receptionist);

        }

        [HttpPost, ActionName("DeleteReceptionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedReceptionist(int id)
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
            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");

            return RedirectToAction(nameof(Receptionists));
        }

        private bool ReceptionistExists(int id)
        {
            return (_context.Receptionists?.Any(e => e.ReceptionistId == id)).GetValueOrDefault();
        }
        //Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.AspNetUsers.ToListAsync();

            var receptionists = await _context.Receptionists.ToListAsync();

            var doctors = await _context.Doctors.ToListAsync();

            ViewData["Receptionists"] = receptionists;
            ViewData["Doctors"] = doctors;

            return View(users);
        }


    }
}
