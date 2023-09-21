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
using System.Globalization;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using Humanizer;
using Microsoft.CSharp.RuntimeBinder;

namespace HOSPITAL2_LAB1.Controllers
{
    [Authorize(Roles = "Receptionist")]
    public class ReceptionistsController : Controller
    {
        private readonly HOSPITAL2Context _context;
        private object patient;

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

        public async Task<IActionResult> Rooms()
        {
            var rooms = await _context.Rooms.ToListAsync();

            return View(rooms);
        }

        [HttpGet]
        public IActionResult CreateRoom()
        {
            return View();
        }

        public IActionResult CreateRoomAlert()
        {
            // Your logic for the CreateRoomAlert page
            return View();
        }

        public async Task<IActionResult> CreateRoom([Bind("RoomId,RoomNumber")] Room room)
        {
            if (ModelState.IsValid)
            {
                if (_context.Rooms.Any(s => s.RoomNumber == room.RoomNumber))
                {
                    ModelState.AddModelError("RoomNumber", "A room with the same number already exists.");
                }
                else
                {
                    _context.Add(room);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("CreateRoomAlert");
                }
            }

            return View(room);
        }



        [HttpGet]
        public async Task<IActionResult> EditRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(int id, [Bind("RoomId,RoomNumber")] Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRoom = await _context.Rooms.FindAsync(id);

                    if (existingRoom == null)
                    {
                        return NotFound();
                    }
                    var roomWithSameNumber = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == room.RoomNumber && r.RoomId != id);

                    if (roomWithSameNumber != null)
                    {
                        ModelState.AddModelError("RoomNumber", "A room with the same number already exists.");
                        return View(room);
                    }

                    existingRoom.RoomNumber = room.RoomNumber;

                    _context.Update(existingRoom);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Rooms));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(room);
        }



        private bool RoomExists(int id)
        {
            return (_context.Rooms?.Any(e => e.RoomId == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> DeleteRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Rooms.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }
       
        [HttpPost, ActionName("DeleteRoom")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedRooms(int id)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(id);
                if (room != null)
                {
                    _context.Rooms.Remove(room);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Rooms));
            }
            catch (SqlException ex) when (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            {
                ModelState.AddModelError("", "Cannot delete the room because it has patients assigned to it.");
                return View("DeleteRoomExc"); // Assuming you have a "DeleteRoom" view
            }
            catch (Exception ex)
            {
                // Handle other exceptions if needed
                ModelState.AddModelError("", "An error occurred while deleting the room.");
                return View("DeleteRoomExc"); // Assuming you have a "DeleteRoom" view
            }
        }



        // Register patient 

        public async Task<IActionResult> Patients()
        {
            var patient = await _context.Patients
                .Include(a => a.User)
                .Include(r => r.RoomNavigation)
                .ToListAsync();

            return View(patient);
        }
        [HttpGet]
        public async Task<IActionResult> EditPatient(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            ViewData["RoomNumber"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");

           
            ViewData["Emails"] = new SelectList(new List<string> { patient.Email });

            return View(patient);
        }



        // POST: Receptionists/EditPatient/5
        // POST: Receptionists/EditPatient/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(int id, [Bind("PatientId,Name,Surname,Gender,Birthday,Email,Phone,Room")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(patient.Name))
            {
                ModelState.AddModelError("Name", "Name is required");
            }
            else if (!Regex.IsMatch(patient.Name, "^[a-zA-Z]+$"))
            {
                ModelState.AddModelError("Name", "Name should only contain letters");
            }

            if (string.IsNullOrWhiteSpace(patient.Surname))
            {
                ModelState.AddModelError("Surname", "Surname is required");
            }
            else if (!Regex.IsMatch(patient.Surname, "^[a-zA-Z]+$"))
            {
                ModelState.AddModelError("Surname", "Surname should only contain letters");
            }

            if (string.IsNullOrWhiteSpace(patient.Gender))
            {
                ModelState.AddModelError("Gender", "Gender is required");
            }
            if (patient.Birthday == null)
            {
                ModelState.AddModelError("Birthday", "Please fill in your birthday");
            }
            else
            {
                var minBirthDate = DateTime.Today.AddYears(-18);

                if (patient.Birthday > minBirthDate)
                {
                    ModelState.AddModelError("Birthday", "You must be at least 18 years old.");
                }
            }

            if (string.IsNullOrWhiteSpace(patient.Email))
            {
                ModelState.AddModelError("Email", "Please select an email");
            }

            if (patient.Phone == null)
            {
                ModelState.AddModelError("Phone", "Phone number is required");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string selectedEmail = patient.Email.Trim().ToLower();

                    var user = await _context.AspNetUsers
                        .Where(u => u.Email.Trim().ToLower() == selectedEmail && u.Email.EndsWith("@patient.com"))
                        .SingleOrDefaultAsync();

                    if (user != null)
                    {
                        patient.UserId = user.Id;

                        if (patient.Room != 1)
                        {
                            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == patient.Room);
                            if (room != null)
                            {
                                var patientsInRoom = await _context.Patients.CountAsync(p => p.Room == patient.Room && p.PatientId != id);
                                if (patientsInRoom >= 3)
                                {
                                    ModelState.AddModelError("", "This room already has 3 patients assigned.");
                                    ViewData["Emails"] = new SelectList(
                                        _context.AspNetUsers
                                            .Where(u => u.Email.EndsWith("@patient.com"))
                                            .ToList(), "Id", "Email");
                                    ViewData["RoomNumber"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");
                                    return View(patient);
                                }
                            }
                        }

                        _context.Update(patient);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Patients));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Selected user not found.");
                    }
                }
                catch (DbUpdateException ex)
                {
                    if (IsUniqueConstraintViolation(ex))
                    {
                        ModelState.AddModelError("Email", "A patient with the same email already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving the patient record.");
                    }
                }
            }

           
            var currentPatientEmail = patient.Email;

           
            ViewData["Emails"] = new SelectList(new List<string> { currentPatientEmail }, currentPatientEmail);

          
            ViewData["RoomNumber"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");

            return View(patient);
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }

        [HttpGet]
        [HttpGet]
        public IActionResult CreatePatient()
        {
            try
            {
                var PatientEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@patient.com")).Select(u => u.Email).ToList();
                SelectList patientEmailsSelectList = new SelectList(PatientEmails);

                var room = _context.Rooms.ToList();
                ViewData["RoomNumber"] = new SelectList(room, "RoomId", "RoomNumber");
                ViewBag.PatientEmails = patientEmailsSelectList;

                return View();
            }
            catch (RuntimeBinderException ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while loading data. Please try again later.";
                // Log the exception or perform additional error handling if needed.
                return View();
            }
        }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreatePatient([Bind("PatientId,Name,Surname,Gender,Birthday,Email,Phone,Room")] Patient patient)
{
    if (string.IsNullOrWhiteSpace(patient.Name))
    {
        ModelState.AddModelError("Name", "Name is required");
    }
    else if (!Regex.IsMatch(patient.Name, "^[a-zA-Z]+$"))
    {
        ModelState.AddModelError("Name", "Name should only contain letters");
    }

    if (string.IsNullOrWhiteSpace(patient.Surname))
    {
        ModelState.AddModelError("Surname", "Surname is required");
    }
    else if (!Regex.IsMatch(patient.Surname, "^[a-zA-Z]+$"))
    {
        ModelState.AddModelError("Surname", "Surname should only contain letters");
    }

    if (string.IsNullOrWhiteSpace(patient.Gender))
    {
        ModelState.AddModelError("Gender", "Gender is required");
    }
    if (patient.Birthday == null)
    {
        ModelState.AddModelError("Birthday", "Please fill in your birthday");
    }
    else
    {
        var minBirthDate = DateTime.Today.AddYears(-18);

        if (patient.Birthday > minBirthDate)
        {
            ModelState.AddModelError("Birthday", "You must be at least 18 years old.");
        }
    }

    if (string.IsNullOrWhiteSpace(patient.Email))
    {
        ModelState.AddModelError("Email", "Please select an email");
    }

    if (patient.Phone == null)
    {
        ModelState.AddModelError("Phone", "Phone number is required");
    }

    if (ModelState.IsValid)
    {
        try
        {
            string selectedEmail = patient.Email;

            var user = await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Email.ToLower() == selectedEmail.ToLower());
            if (user != null)
            {
                patient.UserId = user.Id;

                if (patient.Room != 1)
                {
                    var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == patient.Room);
                    if (room != null)
                    {
                        var patientsInRoom = await _context.Patients.CountAsync(p => p.Room == patient.Room);
                        if (patientsInRoom >= 3)
                        {
                            ModelState.AddModelError("", "This room already has 3 patients assigned.");
                            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Id", "Email");
                            ViewData["RoomNumber"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");
                            return View(patient);
                        }
                    }
                }

                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Patients));
            }
            else
            {
                ModelState.AddModelError("", "Selected user not found.");
            }
        }
        catch (DbUpdateException ex)
        {
            if (IsUniqueConstraintViolation(ex))
            {
                // Handle unique constraint violation (UserId)
                ModelState.AddModelError("Email", "A patient with the same email already exists.");
            }
            else
            {
                // Handle other database-related exceptions
                ModelState.AddModelError("", "An error occurred while saving the patient record.");
            }
        }
    }

            // If the model state is not valid, return to the view with error messages
            ViewBag.PatientEmails = new SelectList(_context.AspNetUsers
                 .Where(u => u.Email.EndsWith("@patient.com"))
                 .Select(u => u.Email)
                 .ToList());
            ViewData["RoomNumber"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");
    return View(patient);
}

private bool IsUniqueConstraintViolation(DbUpdateException ex)
{
    var sqlException = ex.InnerException as SqlException;
    return sqlException?.Number == 2601 || sqlException?.Number == 2627;
}




        //Showing users
        public async Task<IActionResult> Users()
        {
            var users = await _context.AspNetUsers.ToListAsync();

            var patientUsers = users.Where(user => user.Email.EndsWith("@patient.com")).ToList();

            var patients = await _context.Patients.ToListAsync();

            ViewData["Patients"] = patients;

            return View(patientUsers);
        }
        public async Task<IActionResult> SearchPatients(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                var allPatients = await _context.Patients.Include(a => a.User).ToListAsync();
                return View("Patients", allPatients);
            }

            var patient = await _context.Patients
                .Where(d => d.Name.Contains(query) || d.Surname.Contains(query))
                .Include(a => a.User).Include(b => b.RoomNavigation)
                .ToListAsync();

            ViewData["RoomNumber"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");

            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");

            return View("Patients", patient);
        }


        // show appointments
        public async Task<IActionResult> Appointments()
        {

            var appointmentsQuery = _context.Reservations
                .Include(r => r.PatientNavigation)
                .Include(r => r.DoctorNavigation)
                .AsQueryable();
            var appointments = await appointmentsQuery.ToListAsync();
            return View(appointments);

        }



        //Edit
        // GET: Receptionists/EditAppointment/5
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

            ViewBag.DoctorList = new SelectList(_context.Doctors, "DoctorId", "FullName", reservation.Doctor);

            return View(reservation);
        }

        // POST: Receptionists/EditAppointment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAppointment(int id, [Bind("ReservationId,ReservationDate,ReservationTime,Doctor")] Reservation editedReservation)
        {
            if (id != editedReservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (IsTimeSpanValid(editedReservation.ReservationTime))
                    {
                        if (IsDuplicateAppointment(editedReservation))
                        {
                            ModelState.AddModelError("", "This appointment is not available. Please choose another one!");
                        }
                       
                        else
                        {
                            // Update the reservation details
                            var existingReservation = await _context.Reservations.FindAsync(id);
                            existingReservation.ReservationDate = editedReservation.ReservationDate;
                            existingReservation.ReservationTime = editedReservation.ReservationTime;
                            existingReservation.Doctor = editedReservation.Doctor;

                            _context.Update(existingReservation);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Appointments));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid appointment time. Please choose a time between 00:00:00 and 23:59:59.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(editedReservation.ReservationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.DoctorList = new SelectList(_context.Doctors, "DoctorId", "FullName");
            return View(editedReservation);
        }
        private bool IsTimeSpanValid(TimeSpan? timeSpan)
        {
            if (timeSpan.HasValue)
            {
                // Check if the time span is within the valid range
                var minTimeSpan = TimeSpan.Zero;
                var maxTimeSpan = TimeSpan.Parse("23:59:59.9999999");
                return timeSpan >= minTimeSpan && timeSpan <= maxTimeSpan;
            }

            // If the time span is null, consider it invalid
            return false;
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
                return Problem("Entity set 'HOSPITAL2Context.Reservations' is null.");
            }

            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation != null)
            {
                // Remove the reservation
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }

            ViewBag.DoctorList = new SelectList(_context.Doctors, "DoctorId", "FullName");

            return RedirectToAction(nameof(Appointments));
        }
        private bool IsDuplicateAppointment(Reservation editedReservation)
        {
            return _context.Reservations.Any(r =>
                r.ReservationId != editedReservation.ReservationId &&
                r.ReservationDate == editedReservation.ReservationDate &&
                r.ReservationTime == editedReservation.ReservationTime &&
                r.Doctor == editedReservation.Doctor
            );
        }

        public async Task<IActionResult> Payments()
        {
            var paymentsQuery = _context.Payments
                .Include(p => p.PatientNavigation) // Include the related Patient
                .Include(p => p.ReportNavigation) // Include the related Report
                .AsQueryable();

            var payments = await paymentsQuery.ToListAsync();
            return View(payments);
        }

        // Create Payment
        
        [HttpGet]
        public async Task<IActionResult> CreatePayment()
        {
            var patientEmails = _context.Patients
                .Select(patient => new SelectListItem
                {
                    Value = patient.PatientId.ToString(), // Use the appropriate property for the value
                    Text = patient.Email// Use the appropriate property for the text
                })
                .ToList();

            ViewBag.PatientList = new SelectList(patientEmails, "Value", "Text");

            var report = await _context.Reports.ToListAsync();
            ViewBag.ReportList = new SelectList(report, "ReportId", "ReportType");

            return View();
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreatePayment([Bind("PaymentAmount,PaymentDate,Patient,Report")] Payment payment)
{
    if (string.IsNullOrWhiteSpace(payment.PaymentAmount))
    {
        ModelState.AddModelError("PaymentAmount", "Payment amount is required");
    }
    if (payment.PaymentDate == null)
    {
        ModelState.AddModelError("PaymentDate", "Payment date is required");
    }
    if (ModelState.IsValid)
    {
        var existingPatient = await _context.Patients.FindAsync(payment.Patient);
        var existingReport = await _context.Reports.FindAsync(payment.Report);

        if (existingPatient != null && existingReport != null)
        {
            payment.PatientNavigation = existingPatient;
            payment.ReportNavigation = existingReport;

            _context.Add(payment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Payments));
        }
        else
        {
            ModelState.AddModelError("", "Selected Patient or Report not found.");
        }
    }

    var patientEmails = _context.Patients
        .Select(patient => new SelectListItem
        {
            Value = patient.PatientId.ToString(),
            Text = patient.Email
        })
        .ToList();
    ViewBag.PatientList = new SelectList(patientEmails, "Value", "Text");

    var report = await _context.Reports.ToListAsync();
    ViewBag.ReportList = new SelectList(report, "ReportId", "ReportType");

    return View(payment);
}

        /*
        [HttpGet]
        public async Task<IActionResult> CreatePayment()
        {
            var patientEmails = _context.Patients
                .Select(patient => new SelectListItem
                {
                    Value = patient.PatientId.ToString(),
                    Text = patient.Email
                })
                .ToList();

            ViewBag.PatientList = new SelectList(patientEmails, "Value", "Text");

            // Initialize an empty list for reports
            var reports = new List<SelectListItem>();

            // Get the selected patient ID from the query string
            var selectedPatientId = Request.Query["Patient"];

            if (!string.IsNullOrEmpty(selectedPatientId))
            {
                // Fetch reports for the selected patient
                reports = await _context.Reports
                    .Where(r => r.Patient == int.Parse(selectedPatientId))
                    .Select(report => new SelectListItem
                    {
                        Value = report.ReportId.ToString(),
                        Text = report.ReportType
                    })
                    .ToListAsync();
            }

            ViewBag.ReportList = new SelectList(reports, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayment([Bind("PaymentAmount,PaymentDate,Patient,Report")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                // Check if the selected Patient and Report IDs exist in the database
                var existingPatient = await _context.Patients.FindAsync(payment.Patient);
                var existingReport = await _context.Reports.FindAsync(payment.Report);

                if (existingPatient != null && existingReport != null)
                {
                    // Set the navigation properties of the payment entity
                    payment.PatientNavigation = existingPatient;
                    payment.ReportNavigation = existingReport;

                    // Add and save the payment entity
                    _context.Add(payment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Payments));
                }
                else
                {
                    ModelState.AddModelError("", "Selected Patient or Report not found.");
                    // Handle the case where the Patient or Report is not found
                }
            }

            // Repopulate the dropdowns based on the selected patient
            var patientEmails = _context.Patients
                .Select(patient => new SelectListItem
                {
                    Value = patient.PatientId.ToString(),
                    Text = patient.Email
                })
                .ToList();

            ViewBag.PatientList = new SelectList(patientEmails, "Value", "Text");

            // Initialize an empty list for reports
            var reports = new List<SelectListItem>();

            // Get the selected patient ID from the query string
            var selectedPatientId = Request.Query["Patient"];

            if (!string.IsNullOrEmpty(selectedPatientId))
            {
                // Fetch reports for the selected patient
                reports = await _context.Reports
                    .Where(r => r.Patient == int.Parse(selectedPatientId))
                    .Select(report => new SelectListItem
                    {
                        Value = report.ReportId.ToString(),
                        Text = report.ReportType
                    })
                    .ToListAsync();
            }

            ViewBag.ReportList = new SelectList(reports, "Value", "Text");

            return View(payment);
        }
        */


    }

}


