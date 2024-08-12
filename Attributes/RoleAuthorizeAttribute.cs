using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SGCApp.Data;
using SGCApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SGCApp.Attributes
{
    public class RoleAuthorizeAttribute : TypeFilterAttribute
    {
        public RoleAuthorizeAttribute(string permissionName) : base(typeof(RoleAuthorizeFilter))
        {
            Arguments = new object[] { permissionName };
        }
    }

    public class RoleAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly ApplicationDbContext _context;
        private readonly string _permissionName;

        public RoleAuthorizeFilter(ApplicationDbContext context, string permissionName)
        {
            _context = context;
            _permissionName = permissionName;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(context.HttpContext.User);

            if (user == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var roleId = _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .FirstOrDefault();

            if (roleId == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var permission = _context.Permissions
                .FirstOrDefault(p => p.Name == _permissionName);

            if (permission == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasPermission = _context.RolePermissions
                .Any(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

            if (!hasPermission)
            {
                // Usa HttpContext.Items para agregar la bandera del modal
                context.HttpContext.Items["ShowAccessDeniedModal"] = true;

                context.Result = new ViewResult
                {
                    ViewName = (context.Result as ViewResult)?.ViewName ?? "Index"
                };
            }
        }
    }
}
