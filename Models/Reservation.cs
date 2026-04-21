using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string CommonArea { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
