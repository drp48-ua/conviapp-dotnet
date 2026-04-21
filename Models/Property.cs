using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConviAppWeb.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Location { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Rooms { get; set; }
        public string? ImageFile { get; set; } // e.g. "piso_madrid.jpg"

        public ICollection<User> Tenants { get; set; } = new List<User>();
    }
}
