using Microsoft.AspNetCore.Identity;

namespace SGCApp.Models
{
    public class ProcedureVersion
    {
        public int Id { get; set; }
        public int ProcedureId { get; set; }
        public string VersionNumber { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ModifiedBy { get; set; }

        public Procedure Procedure { get; set; }
        public ApplicationUser ModifiedByUser { get; set; } // Usa ApplicationUser en lugar de IdentityUser
    }
}
