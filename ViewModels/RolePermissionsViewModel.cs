using Microsoft.AspNetCore.Identity;
using SGCApp.Models;
using System.Collections.Generic;

namespace SGCApp.ViewModels
{
    public class RolePermissionsViewModel
    {
        public IdentityRole Role { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
