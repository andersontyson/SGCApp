using System;
using System.ComponentModel.DataAnnotations;

namespace SGCApp.Models
{
    public class ProcedureViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Título")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Code { get; set; }

        [Display(Name = "Versión")]
        public string Version { get; set; }

        // Ruta del archivo del procedimiento
        public string FilePath { get; set; }

        // Este campo solo se utilizará para mostrar el contenido del archivo durante la edición
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        public IFormFile FileUpload { get; set; }
    }
}
