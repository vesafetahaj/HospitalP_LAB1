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

    }



    // Register patient 
    /* public async Task<IActionResult> Patients()
     {
         var doctors = await _context.Patients
             .Include(a => a.User)
             .Include(r => r.SpecializationNavigation)
             .ToListAsync();

         return View(patients);
     }
     public async Task<IActionResult> EditPatient(int? id)
     {
         if (id == null || _context.Patients == null)
         {
             return NotFound();
         }

         var doctor = await _context.Patients.FindAsync(id);
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
         var doctorEmails = _context.AspNetUsers.Where(u => u.Email.EndsWith("@doctor.com")).Select(u => u.Email).ToList();
         SelectList doctorEmailsSelectList = new SelectList(doctorEmails);

         var specializations = _context.Specializations.ToList();
         ViewData["Name"] = new SelectList(specializations, "SpecializationId", "Name");

         ViewData["Emails"] = doctorEmailsSelectList;

         return View();
     }


     [HttpPost]
     [ValidateAntiForgeryToken]
     public async Task<IActionResult> CreateDoctor([Bind("DoctorId,Name,Surname,Education,Specialization,Email,PhotoUrl")] Doctor doctor)
     {
         if (ModelState.IsValid)
         {
             // Check if a doctor with the same email already exists
             if (_context.Doctors.Any(d => d.Email == doctor.Email))
             {
                 ModelState.AddModelError("Email", "This doctor already exists.");
                 ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");
                 ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");
                 return View(doctor);
             }

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
             .Include(a => a.User).Include(b => b.SpecializationNavigation)
             .ToListAsync();

         // Populate the ViewBag.Name for the Specializations dropdown
         ViewData["Name"] = new SelectList(_context.Specializations, "SpecializationId", "Name");

         // Populate the ViewBag.Emails for the Emails dropdown
         ViewData["Emails"] = new SelectList(_context.AspNetUsers, "Email", "Email");

         return View("Doctors", doctors);
     }
    */
}
