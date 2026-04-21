using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Name { get; set; }

        public string Role { get; set; } // Enterprise, Profesional, Basico

        public int? PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
