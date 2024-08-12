// Agrega o modifica el modelo DocumentVersion.cs en la carpeta Models

using SGCApp.Models;
using System;

namespace SGCApp.Models
{
    public class DocumentVersion
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string VersionNumber { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ModifiedBy { get; set; }

        public Document Document { get; set; }
        public ApplicationUser ModifiedByUser { get; set; }
    }
}