using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using SGCApp.Models;

namespace SGCApp.ViewModels
{
    public class EditRolePermissionsViewModel
    {
        public string RoleId { get; set; }
        public IdentityRole Role { get; set; }  // Añadido para incluir el rol
        public List<Permission> AllPermissions { get; set; }
        public List<int> SelectedPermissionIds { get; set; }
    }
}
