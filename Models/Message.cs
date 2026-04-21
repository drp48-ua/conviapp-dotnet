using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public int SenderId { get; set; }
        public User Sender { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
