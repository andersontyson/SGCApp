using Microsoft.AspNetCore.Identity;
using SGCApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SGCApp.ViewModels

{
    public class RegisterViewModel

    {
        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }

       
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Nombre Completo")]
        public string NormalizedUserName { get; set; }

        //[Required]
        [Phone]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación de la contraseña no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "Departament")]
        public int DepartamentId { get; set; }

        public List<IdentityRole> Roles { get; set; }
        public List<Departament> Departaments { get; set; }

        public string ReturnUrl { get; set; }

        //[Required]
        //[EmailAddress]
        //public string Email { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm Password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }
        //[Required]
        //[Display(Name = "Role")]
        //public string Role { get; set; }

        //[Required]
        //[Display(Name = "Departament")]
        //public int DepartamentId { get; set; }
        //public List<IdentityRole> Roles { get; set; }
        //public List<Departament> Departaments { get; set; }

    }
}
