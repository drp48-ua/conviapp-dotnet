using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConviAppWeb.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public int PayerId { get; set; }
        public User Payer { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
