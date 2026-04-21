using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        public bool IsCompleted { get; set; }
        public string Priority { get; set; } // Low, Medium, High

        public int? AssigneeId { get; set; }
        public User Assignee { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
