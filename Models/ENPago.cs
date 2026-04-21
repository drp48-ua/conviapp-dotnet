using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENPago — Entidad de Negocio para pagos de alquiler.
    /// Capa de lógica de negocio (Entrega 3 - Dani).
    /// </summary>
    public class ENPago
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El importe debe ser mayor que 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio")]
        public string Method { get; set; } // transferencia, efectivo, tarjeta, bizum

        [Required]
        public string Status { get; set; } = "pendiente"; // pagado, pendiente, rechazado

        public string? Concept { get; set; } // Alquiler Abril, Depósito, etc.

        public string? Reference { get; set; } // Referencia bancaria

        // FK → Contrato
        public int ContratoId { get; set; }
        public ENContrato Contrato { get; set; }

        // FK → User (quien paga)
        public int UserId { get; set; }
        public User User { get; set; }

        // ─── Business Logic Methods ───
        public bool IsPaid() => Status == "pagado";
        public bool IsOverdue() => Status == "pendiente" && Date < DateTime.Now;
    }
}
