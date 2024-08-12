using Microsoft.EntityFrameworkCore;
using SGCApp.Models;

namespace SGCApp.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string CurrentVersion { get; set; } // Nuevo campo
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ModifiedBy { get; set; }// Agrega este campo para almacenar el usuario que modificó el documento

    }
    namespace SGCApp.Data
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Document> Documents { get; set; }
            public DbSet<DocumentVersion> DocumentVersions { get; set; }
        }
    }


}

