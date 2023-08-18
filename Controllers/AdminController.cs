using HOSPITAL2_LAB1.Data;
using HOSPITAL2_LAB1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOSPITAL2_LAB1.Controllers
{
    public class AdminController : Controller
    {
        private readonly HOSPITAL2Context _context; // Replace with your DbContext

        public AdminController(HOSPITAL2Context context)
        {
            _context = context;
        }
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Administrator model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get the currently logged-in user's ID
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Assign the UserID and save the data to the database
                    model.UserId = userId;
                   
                    _context.Administrators.Add(model);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch
            {
                return View(model);
            }
        }
        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
