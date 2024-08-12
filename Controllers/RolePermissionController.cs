// RolePermissionController.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGCApp.Attributes;
using SGCApp.Data;
using SGCApp.Models;
using SGCApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace SGCApp.Controllers
{
    [Authorize]
    public class RolePermissionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolePermissionController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }
        [RoleAuthorize("RelaPermiso")]

        public async Task<IActionResult> Index()
        {
            var rolePermissions = await _context.RolePermissions
        .Include(rp => rp.Permission)  // Asegurarse de incluir la entidad Permission
        .GroupBy(rp => rp.RoleId)
        .Select(group => new RolePermissionsViewModel
        {
            Role = _roleManager.Roles.FirstOrDefault(r => r.Id == group.Key),
            Permissions = group.Select(rp => rp.Permission).ToList()
        })
        .ToListAsync();

            return View(rolePermissions);
        }

   
        // GET: RolePermission/Create
        [HttpGet]
        [RoleAuthorize("RelaPermiso")]
        public async Task<IActionResult> Create()
        {
            // Obtener todos los roles
            var allRoles = await _roleManager.Roles.ToListAsync();

            // Obtener roles ya asignados
            var assignedRoleIds = await _context.RolePermissions
                .Select(rp => rp.RoleId)
                .Distinct()
                .ToListAsync();

            // Filtrar roles no asignados
            var availableRoles = allRoles
                .Where(role => !assignedRoleIds.Contains(role.Id))
                .ToList();

            // Preparar ViewBag para dropdowns
            ViewBag.Roles = new SelectList(availableRoles, "Id", "Name");

            // Obtener permisos agrupados por categoría
            var permissions = await _context.Permissions.ToListAsync();
            ViewBag.Permissions = permissions
                .GroupBy(p => p.Category)
                .ToList();

            return View();
        }


        // POST: RolePermission/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("RelaPermiso")]
        public async Task<IActionResult> Create(string roleId, int[] selectedPermissions)
        {
            if (ModelState.IsValid)
            {
                // Crear asignaciones de permisos para el rol seleccionado
                foreach (var permissionId in selectedPermissions)
                {
                    var rolePermission = new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId
                    };
                    _context.Add(rolePermission);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-cargar ViewBag en caso de error
            var allRoles = await _roleManager.Roles.ToListAsync();
            var assignedRoleIds = await _context.RolePermissions
                .Select(rp => rp.RoleId)
                .Distinct()
                .ToListAsync();
            var availableRoles = allRoles
                .Where(role => !assignedRoleIds.Contains(role.Id))
                .ToList();
            ViewBag.Roles = new SelectList(availableRoles, "Id", "Name");
            ViewBag.Permissions = await _context.Permissions.ToListAsync();

            return View();
        }


        [RoleAuthorize("EditaRelaPermiso")]
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

            var permissions = await _context.Permissions.ToListAsync();
            var selectedPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var viewModel = new EditRolePermissionsViewModel
            {
                RoleId = id,
                Role = role,  // Aquí asignas el rol al modelo
                SelectedPermissionIds = selectedPermissions,
                AllPermissions = permissions
            };

            return View(viewModel);
        }

        // POST: RolePermission/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("EditaRelaPermiso")]
        public async Task<IActionResult> Edit(string id, int[] selectedPermissions)
        {
            if (id == null)
            {
                return NotFound();
            }

            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == id)
                .ToListAsync();

            // Eliminar permisos existentes
            _context.RolePermissions.RemoveRange(existingPermissions);

            // Agregar permisos seleccionados
            foreach (var permissionId in selectedPermissions)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = id,
                    PermissionId = permissionId
                };
                _context.Add(rolePermission);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
