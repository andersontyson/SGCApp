using System.Collections.Generic;

namespace SGCApp.Models
{
    public class Permission
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }  // Nueva propiedad de categoría
        public string Description { get; set; }  // Nueva propiedad de categoría

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        
    }
}
