    using Microsoft.AspNetCore.Identity;

    namespace SGCApp.Models
    {
        public class ApplicationUser : IdentityUser
        {
        public int DepartamentId { get; set; }
          
        public virtual Departament Departament { get; set; }

        public string NormalizedUserName { get; set; }
        public string PhoneNumber { get; set; }
         

        }
    }
