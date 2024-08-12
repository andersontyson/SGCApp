// ViewModels/UserViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace SGCApp.ViewModels
{
    public class UserViewModel
    {

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int DepartamentId { get; set; }
        public string DepartamentName { get; set; }  // Agregar esta propiedad      public string DepartamentName { get; set; }  // Agregar esta propiedad
        public string NormalizedUserName { get; set; }
        public string PhoneNumber { get; set; }
    }
}

