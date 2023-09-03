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

        [HttpPost]
        [ValidateAntiForgeryToken]

       
        public async Task<IActionResult> CreateRoom([Bind("RoomId,RoomNumber")] Room room)
        {
            if (ModelState.IsValid)
            {
                if (_context.Rooms.Any(s => s.RoomNumber == room.RoomNumber))
                {
                    ModelState.AddModelError("RoomNumber", "A room with the same number already exists.");
                    return View(room);
                }
                else
                {
                    _context.Add(room);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Rooms));
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
                    // Retrieve the existing specialization from the DbContext
                    var existingRoom = await _context.Rooms.FindAsync(id);

                    // Update the properties of the existing specialization with the values from the binding model
                    existingRoom.RoomNumber = room.RoomNumber;

                    // Save the changes to the DbContext
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
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Rooms));
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

            var patientsEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@patient.com")).Select(u => u.Email).ToList();
            ViewData["Emails"] = new SelectList(patientsEmails);
            ViewData["RoomNumber"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");

            return View(patient);
        }

        // POST: Receptionists/EditPatient/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(int id, [Bind("PatientId,Name,Surname,Gender,Birthday,Email,Phone,Room")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the selected email from the patient object
                    string selectedEmail = patient.Email;

                    // Find the user with the selected email in the AspNetUsers table
                    var user = await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Email == selectedEmail);

                    if (user != null)
                    {
                        // Set the UserId property of the patient entity
                        patient.UserId = user.Id;

                        _context.Update(patient);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Patients));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Selected user not found.");
                    }
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
            }

            // Retrieve the same list of patient emails and room numbers as in the GET action
            var patientsEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@patient.com")).Select(u => u.Email).ToList();
            ViewData["Emails"] = new SelectList(patientsEmails);
            ViewData["RoomNumber"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");

            return View(patient);

          
        }




        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }

        public IActionResult CreatePatient()
             {
                 var PatientEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@patient.com")).Select(u => u.Email).ToList();
                 SelectList patientEmailsSelectList = new SelectList(PatientEmails);

                 var room = _context.Rooms.ToList();
                 ViewData["RoomNumber"] = new SelectList(room, "RoomId", "RoomNumber");

                 ViewData["Emails"] = patientEmailsSelectList;

                 return View();
             }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient([Bind("PatientId,Name,Surname,Gender,Birthday,Email,Phone,Room")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                // Get the selected email from the patient object
                string selectedEmail = patient.Email;

                // Find the user with the selected email in the AspNetUsers table
                var user = await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Email == selectedEmail);

                if (user != null)
                {
                    // Set the UserId property of the patient entity
                    patient.UserId = user.Id;

                    // Add and save the patient entity
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Patients));
                }
                else
                {
                    ModelState.AddModelError("", "Selected user not found.");
                    // Handle the case where the user is not found
                }
            }

            ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Id", "Email");
            ViewData["RoomNumber"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");
            return View(patient);
        }

        public async Task<IActionResult> SearchPatients(string query)
             {
                 if (string.IsNullOrEmpty(query))
                 {
                     // If the search string is empty or null, return all doctors
                     var allPatients = await _context.Patients.Include(a => a.User).ToListAsync();
                     return View("Patients", allPatients);
                 }

                 // Search for doctors whose name or rooom contains the search query
                 var patient = await _context.Patients
                     .Where(d => d.Name.Contains(query) || d.Surname.Contains(query))
                     .Include(a => a.User).Include(b => b.RoomNavigation)
                     .ToListAsync();

                 // Populate the ViewBag.Name for the Room dropdown
                 ViewData["RoomNumber"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");

                 // Populate the ViewBag.Emails for the Emails dropdown
                 ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");

                 return View("Patients", patient);
             }




        //Showing users
        public async Task<IActionResult> Users()
        {
            // Retrieve all users from the database
            var users = await _context.AspNetUsers.ToListAsync();

            // Filter the list to only include patients
            var patientUsers = users.Where(user => user.Email.EndsWith("@patient.com")).ToList();

            // Retrieve receptionist information
            var patients = await _context.Patients.ToListAsync();

            // Store the receptionist and doctor information in ViewData
            ViewData["Patients"] = patients;

            // Pass the filtered patient user list to the view
            return View(patientUsers);
        }


        // Creating an appointment
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
                    var existingAppointment = await _context.Reservations
                        .FirstOrDefaultAsync(a => a.ReservationDate == reservation.ReservationDate &&
                                                  a.ReservationTime == reservation.ReservationTime &&
                                                  a.Doctor == reservation.Doctor);

                    if (existingAppointment != null)
                    {
                        ModelState.AddModelError("", "The appointment is not available.Please choose another one!");
                    }
                    else
                    {
                        reservation.Patient = patient.PatientId;
                        _context.Add(reservation);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Appointments));
                    }
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
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

            if (patient != null)
            {
                var appointments = await _context.Reservations
                    .Include(a => a.DoctorNavigation)
                    .Where(a => a.Patient == patient.PatientId)
                    .ToListAsync();

                return View(appointments);
            }

            // Handle case where the patient is not found
            return NotFound();
        }



        //Edit
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
                    if (IsDuplicateAppointment(editedReservation))
                    {
                        ModelState.AddModelError("", "This appointment is not available.Please choose another one!");
                    }
                    else
                    {
                        var existingReservation = await _context.Reservations.FindAsync(id);
                        existingReservation.ReservationDate = editedReservation.ReservationDate;
                        existingReservation.ReservationTime = editedReservation.ReservationTime;
                        existingReservation.Doctor = editedReservation.Doctor;
                        _context.Update(existingReservation);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Appointments));
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
        private bool IsDuplicateAppointment(Reservation editedReservation)
        {
            return _context.Reservations.Any(r =>
                r.ReservationId != editedReservation.ReservationId &&
                r.ReservationDate == editedReservation.ReservationDate &&
                r.ReservationTime == editedReservation.ReservationTime &&
                r.Doctor == editedReservation.Doctor
            );
        }

    }

}

