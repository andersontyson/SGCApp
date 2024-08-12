using System;
using System.ComponentModel.DataAnnotations;

namespace SGCApp.Models
{
    public class Record
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
