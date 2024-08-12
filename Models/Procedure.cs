// Models/Procedure.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace SGCApp.Models
{
    public class Procedure
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string CurrentVersion { get; set; } // Nuevo campo
        public ApplicationUser ModifiedByUser { get; set; }

        public ICollection<ProcedureVersion> ProcedureVersions { get; set; } // Relación con versiones

    }

}