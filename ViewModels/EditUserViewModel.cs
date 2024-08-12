using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using SGCApp.Models;

namespace SGCApp.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "Nombre Completo")]
        public string NormalizedUserName { get; set; }

        [Required]
        [Display(Name = "Departamento")]
        public int DepartamentId { get; set; }

        public List<IdentityRole> Roles { get; set; }

        public List<Departament> Departaments { get; set; }
    }
}
