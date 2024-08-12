using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGCApp.Attributes;
using SGCApp.Data;
using SGCApp.Models;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Authorization;


namespace SGCApp.Controllers
{
    [Authorize]
    public class PermissionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public PermissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            var permissions = _context.Permissions
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToPagedList(pageNumber, PageSize);
            return View(permissions);
        }
        [RoleAuthorize("CreaPermiso")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("CreaPermiso")]
        public async Task<IActionResult> Create([Bind("Name,Category,Description")] Permission permission)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(permission);
        }

        [RoleAuthorize("EditaPermiso")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }
            return View(permission);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("EditaPermiso")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,Description")] Permission permission)
        {
            if (id != permission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermissionExists(permission.Id))
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
            return View(permission);
        }

        private bool PermissionExists(int id)
        {
            return _context.Permissions.Any(e => e.Id == id);
        }
    }
}
