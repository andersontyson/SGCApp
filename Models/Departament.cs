// Models/Departament.cs
namespace SGCApp.Models
{
    public class Departament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Agrega propiedades adicionales aquí si es necesario

        // Navegation property to establish the relationship
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
