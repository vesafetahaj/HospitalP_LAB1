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
using System.Composition;
using System.Numerics;

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
        private async Task<bool> HasConflictingAppointment(Reservation reservation)
        {
            var existingAppointments = await _context.Reservations
                .Where(a => a.Doctor == reservation.Doctor)
                .ToListAsync();

            foreach (var existingAppointment in existingAppointments)
            {
                DateTimeOffset reservationDateTime = new DateTimeOffset(
                    reservation.ReservationDate.Value,
                    reservation.ReservationTime.Value
                );

                DateTimeOffset existingAppointmentDateTime = new DateTimeOffset(
                    existingAppointment.ReservationDate.Value,
                    existingAppointment.ReservationTime.Value
                );

                var timeDifferenceMinutes = (reservationDateTime - existingAppointmentDateTime).TotalMinutes;

                if (Math.Abs(timeDifferenceMinutes) < 30)
                {
                    return true;
                }
            }

            return false;
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
            if (reservation.ReservationDate == null)
            {
                ModelState.AddModelError("ReservationDate", "Reservation date is required");
            }

            if (reservation.ReservationTime == null)
            {
                ModelState.AddModelError("ReservationTime", "Reservation time is required");
            }
            if (reservation.Doctor == null)
            {
                ModelState.AddModelError("Doctor", "Doctor is required.");
            }
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
                        ModelState.AddModelError("", "The appointment is not available. Please choose another one!");
                    }else if (await HasConflictingAppointment(reservation))
                    {
                        ModelState.AddModelError("", "Unavailable appointment!");
                    }
                    else
                    {
                        // Set the patient ID in the reservation
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
        public async Task<IActionResult> EditAppointment(int id, [Bind("ReservationId,ReservationDate,ReservationTime,Doctor")] Reservation editedReservation)
        {
            if (editedReservation.ReservationDate == null)
            {
                ModelState.AddModelError("ReservationDate", "Reservation date is required");
            }

            if (editedReservation.ReservationTime == null)
            {
                ModelState.AddModelError("ReservationTime", "Reservation time is required");
            }
            if (editedReservation.Doctor == null)
            {
                ModelState.AddModelError("Doctor", "Doctor is required.");
            }
            if (id != editedReservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingReservation = await _context.Reservations.FindAsync(id);

                    if (IsDuplicateAppointment(editedReservation))
                    {
                        ModelState.AddModelError("", "This appointment is not available. Please choose another one!");
                    }
                    else if (await HasConflictingAppointment(editedReservation))
                    {
                        ModelState.AddModelError("", "Unavailable appointment!");
                    }
                    else
                    {
                        // Update the reservation details
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

        
        //complaints
        public async Task<IActionResult> Complaints()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

            if (patient != null)
            {
                var complaints = await _context.Complaints

                     .Where(a => a.Patient == patient.PatientId)
                    .ToListAsync();

                return View(complaints);
            }

            // Handle case where the patient is not found
            return NotFound();
        }

        public IActionResult CreateComplaint()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComplaint([Bind("ComplaintId,ComplaintDate,ComplaintDetails")] Complaint complaint)
        {

            if (complaint.ComplaintDate == null)
            {
                ModelState.AddModelError("ComplaintDate", "Complaint date is required");
            }

            if (complaint.ComplaintDetails == null)
            {
                ModelState.AddModelError("ComplaintDetails", "Complaint detail is required");
            }
          
            if (ModelState.IsValid)
            {
                /* if (_context.Complaints.Any(s => s.Name == complaint.Name))
                 {
                     ModelState.AddModelError("Name", "A complaint with the same name already exists.");
                     return View(complaint);
                 }*/

                string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var patient = await _context.Patients.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

                if (patient != null)
                {
                    complaint.Patient = patient.PatientId;

                    _context.Add(complaint);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Complaints));

                }
                else
                {
                    ModelState.AddModelError("", "You have to provide personal info first.");
                }
            }

            return View(complaint);
        }


        public async Task<IActionResult> EditComplaint(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }

            return View(complaint);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComplaint(int id, [Bind("ComplaintId,ComplaintDate,ComplaintDetails,Patient")] Complaint complaint)
        {

            if (complaint.ComplaintDate == null)
            {
                ModelState.AddModelError("ComplaintDate", "Complaint date is required");
            }

            if (complaint.ComplaintDetails == null)
            {
                ModelState.AddModelError("ComplaintDetails", "Complaint detail is required");
            }
            if (id != complaint.ComplaintId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   
                    var existingComplaint = await _context.Complaints.FindAsync(id);

                   
                    existingComplaint.ComplaintDate = complaint.ComplaintDate;
                    existingComplaint.ComplaintDetails = complaint.ComplaintDetails;


                   
                    _context.Update(existingComplaint);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Complaints));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComplaintExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(complaint);
        }
        private bool ComplaintExists(int id)
        {
            return (_context.Complaints?.Any(e => e.ComplaintId == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> DeleteComplaint(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }

            return View(complaint);
        }

        [HttpPost, ActionName("DeleteComplaint")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedComplaint(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint != null)
            {
                _context.Complaints.Remove(complaint);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Complaints));
        }




        //contact forma
        public async Task<IActionResult> ContactForms()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

            if (patient != null)
            {
                var contactform = await _context.ContactForms
                   .Where(a => a.Patient == patient.PatientId)
                    .ToListAsync();

                return View(contactform);
            }

        
            return NotFound();
        }

        public IActionResult CreateMessage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMessage([Bind("ContactId,Subject,Message")] ContactForm contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Subject))
            {
                ModelState.AddModelError("Subject", "A subject is required.");
            }
            if (contact.Message == null)
            {
                ModelState.AddModelError("Message", "A message is required.");
            }
         
            if (ModelState.IsValid)
            {

                string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var patient = await _context.Patients.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

                if (patient != null)
                {
                    contact.Patient = patient.PatientId;

                    _context.Add(contact);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ContactForms));

                }
                else
                {
                    ModelState.AddModelError("", "You have to provide personal info first.");
                }
            }

            return View(contact);
        }


        public async Task<IActionResult> EditMessage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.ContactForms.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMessage(int id, [Bind("ContactId,Subject,Message,Patient")] ContactForm contact)
        {

            if (string.IsNullOrWhiteSpace(contact.Subject))
            {
                ModelState.AddModelError("Subject", "A subject is required.");
            }
            if (contact.Message == null)
            {
                ModelState.AddModelError("Message", "A message is required.");
            }
            if (id != contact.ContactId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing specialization from the DbContext
                    var existingContact = await _context.ContactForms.FindAsync(id);

                    // Update the properties of the existing specialization with the values from the binding model
                    existingContact.Subject = contact.Subject;
                    existingContact.Message = contact.Message;


                    // Save the changes to the DbContext
                    _context.Update(existingContact);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(ContactForms));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(contact);
        }
        private bool MessageExists(int id)
        {
            return (_context.ContactForms?.Any(e => e.ContactId == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> DeleteMessage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.ContactForms.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        [HttpPost, ActionName("DeleteMessage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedContact(int id)
        {
            var contact = await _context.ContactForms.FindAsync(id);
            if (contact != null)
            {
                _context.ContactForms.Remove(contact);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ContactForms));
        }

        //shfaqja e raporteve nga doktori tek pacienti
        public async Task<IActionResult> Reports()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

            if (patient != null)
            {
                var raport = await _context.Reports
                    .Include(a => a.DoctorNavigation)
                    .Where(a => a.Patient == patient.PatientId)
                    .ToListAsync();

                return View(raport);
            }

            // Handle case where the patient is not found
            return NotFound();
        }

        //shfaqja e pagesave tek pacienti
        public async Task<IActionResult> Payments()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

            if (patient != null)
            {
                var payments = await _context.Payments
                    .Include(a => a.ReportNavigation)
                    .Where(a => a.Patient == patient.PatientId)
                    .ToListAsync();

                return View(payments);
            }

            // Handle case where the patient is not found
            return NotFound();
        }

    }
}
