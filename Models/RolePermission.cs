using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGCApp.Models;

namespace SGCApp.Models
{
    public class RolePermission
    {
        public int Id { get; set; }

        public string RoleId { get; set; } // Clave externa hacia IdentityRole

        public IdentityRole Role { get; set; }

        public int PermissionId { get; set; } // Clave externa hacia Permission

        public Permission Permission { get; set; }
    }

    namespace SGCApp.Models
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Permission> Permissions { get; set; }
            public DbSet<RolePermission> RolePermissions { get; set; }

        }
    }

}
