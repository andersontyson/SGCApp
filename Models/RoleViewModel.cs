﻿using System.ComponentModel.DataAnnotations;

namespace SGCApp.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
