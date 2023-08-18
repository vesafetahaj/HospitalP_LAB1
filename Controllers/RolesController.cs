using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_System_Management.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _manager;
        public RolesController(RoleManager<IdentityRole>roleManager) {
            _manager= roleManager;
        
        }
        public IActionResult Index()
        {
            var roles = _manager.Roles;
            return View(roles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(IdentityRole role)

        {
            //nese roli ekziston
            if (!_manager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                _manager.CreateAsync(new IdentityRole(role.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var role = _manager.FindByIdAsync(id).GetAwaiter().GetResult();
            if (role != null)
            {
                return View(role);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var existingRole = _manager.FindByIdAsync(role.Id).GetAwaiter().GetResult();
                if (existingRole != null)
                {
                    existingRole.Name = role.Name;
                    var result = _manager.UpdateAsync(existingRole).GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Role not found.");
                }
            }

            return View(role);
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            var role = _manager.FindByIdAsync(id).GetAwaiter().GetResult();
            if (role != null)
            {
                return View(role);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(string id)
        {
            var role = _manager.FindByIdAsync(id).GetAwaiter().GetResult();
            if (role != null)
            {
                var result = _manager.DeleteAsync(role).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}
  