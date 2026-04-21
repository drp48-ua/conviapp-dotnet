using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    public class Incident
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // abierta, en proceso, cerrada
        public DateTime DateReported { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
