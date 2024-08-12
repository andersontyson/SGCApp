using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGCApp.Models;
using SGCApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using SGCApp.Data;
using System.Diagnostics;
using SGCApp.Attributes;
using X.PagedList.Extensions;
using X.PagedList;
using X.PagedList.Mvc.Core;


namespace SGCApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
            private const int PageSize = 8;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        [RoleAuthorize("Veruser")]
        public IActionResult Index(int? page)
        {
            var users = _userManager.Users.Select(u => new UserViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                NormalizedUserName = u.NormalizedUserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault(),
                DepartamentId = u.DepartamentId,
                DepartamentName = _context.Departaments
                                         .Where(d => d.Id == u.DepartamentId)
                                         .Select(d => d.Name)
                                         .FirstOrDefault()
            }).ToList();

                int pageNumber = page ?? 1;
                var pagedUsers = users.ToPagedList(pageNumber, PageSize);

            return View(pagedUsers);
        }

        [HttpGet]
        [RoleAuthorize("CreaUser")]
        public IActionResult Register(string returnUrl = null)
        {
            var model = new RegisterViewModel
            {
                Roles = _roleManager.Roles.ToList(),
                Departaments = _context.Departaments.ToList(),
                ReturnUrl = returnUrl ?? "/"
            };
            return View(model);
        }

        [HttpPost]
        [RoleAuthorize("CreaUser")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    NormalizedUserName = model.NormalizedUserName.ToUpper(),
                    PhoneNumber = model.PhoneNumber,
                    DepartamentId = model.DepartamentId
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    }

                    await _userManager.AddToRoleAsync(user, model.Role);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.Roles = _roleManager.Roles.ToList();
            model.Departaments = _context.Departaments.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [RoleAuthorize("EditUser")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Debug.WriteLine($"Edit action received id: {id}");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                NormalizedUserName = user.NormalizedUserName,
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                DepartamentId = user.DepartamentId,
                Roles = _roleManager.Roles.ToList(),
                Departaments = _context.Departaments.ToList()
            };

            return View(model);
        }
        public IActionResult AccessDenied()
        {
            ViewBag.Message = "No tienes permiso para acceder a esta sección.";
            return View();
        }

        [HttpPost]
        [RoleAuthorize("EditUser")]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            //if (ModelState.IsValid)
            //{
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.DepartamentId = model.DepartamentId;
                user.NormalizedUserName = model.NormalizedUserName.ToUpper();

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.FirstOrDefault() != model.Role)
                    {
                        if (!await _roleManager.RoleExistsAsync(model.Role))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(model.Role));
                        }

                        await _userManager.RemoveFromRolesAsync(user, roles);
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            //}

            model.Roles = _roleManager.Roles.ToList();
            model.Departaments = _context.Departaments.ToList();
            return View(model);
        }
    }
}
