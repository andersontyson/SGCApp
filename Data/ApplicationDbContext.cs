using Microsoft.EntityFrameworkCore;
using SGCApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SGCApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<ProcedureVersion> ProcedureVersions { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        public DbSet<Departament> Departaments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración de RolePermission
            builder.Entity<RolePermission>()
                .HasKey(rp => rp.Id);

            builder.Entity<RolePermission>()
                .Property(rp => rp.Id)
                .ValueGeneratedOnAdd();

            // Relación con IdentityRole
            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleId)
                .IsRequired();

            // Relación con Permission
            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .IsRequired();

            // Relación de Departamento con Usuarios
            builder.Entity<Departament>()
                .HasMany(d => d.Users)
                .WithOne(u => u.Departament)
                .HasForeignKey(u => u.DepartamentId);

            // Relación de ProcedureVersion con ApplicationUser
            builder.Entity<ProcedureVersion>()
                .HasOne(pv => pv.ModifiedByUser)
                .WithMany()
                .HasForeignKey(pv => pv.ModifiedBy) // Asegúrate de que 'ModifiedBy' en ProcedureVersion sea del tipo string
                .HasPrincipalKey(u => u.Id);  // 'Id' en ApplicationUser es un string


            // Relación de DocumentVersion con ApplicationUser
            builder.Entity<DocumentVersion>()
                .HasOne(pv => pv.ModifiedByUser)
                .WithMany()
                .HasForeignKey(pv => pv.ModifiedBy) // Asegúrate de que 'ModifiedBy' en ProcedureVersion sea del tipo string
                .HasPrincipalKey(u => u.Id);  // 'Id' en ApplicationUser es un string

            // Configuración adicional de relaciones si es necesario
        }
    }
}
