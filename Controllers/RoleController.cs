using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGCApp.Data;
using SGCApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using SGCApp.Attributes;
using Microsoft.AspNetCore.Authorization;


namespace SGCApp.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _roleManager.Roles.ToListAsync());
        }
        [RoleAuthorize("CreaRol")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("CreaRol")]
        public async Task<IActionResult> Create([Bind("Name")] RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid || !string.IsNullOrWhiteSpace(roleViewModel.Name))
            {
                var role = new IdentityRole { Name = roleViewModel.Name };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(roleViewModel);
        }

        [RoleAuthorize("EditaRol")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var roleViewModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(roleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("EditaRol")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] RoleViewModel roleViewModel)
        {
            if (id != roleViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRole = await _roleManager.FindByIdAsync(id);
                    existingRole.Name = roleViewModel.Name;
                    var result = await _roleManager.UpdateAsync(existingRole);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(roleViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(roleViewModel);
        }

        private bool RoleExists(string id)
        {
            return _roleManager.Roles.Any(e => e.Id == id);
        }
    }
}
