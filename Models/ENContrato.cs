using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENContrato — Entidad de Negocio para contratos de arrendamiento.
    /// Capa de lógica de negocio (Entrega 3 - Dani).
    /// </summary>
    public class ENContrato
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de contrato es obligatorio")]
        public string Type { get; set; } // arrendamiento, temporal, subarriendo

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "La renta debe ser positiva")]
        public decimal MonthlyRent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositAmount { get; set; }

        [Required]
        public string Status { get; set; } = "activo"; // activo, vencido, cancelado

        public string? Notes { get; set; }

        // FK → Property
        public int PropertyId { get; set; }
        public Property Property { get; set; }

        // FK → User (inquilino)
        public int UserId { get; set; }
        public User User { get; set; }

        // Navigation: Pagos asociados
        public ICollection<ENPago> Pagos { get; set; } = new List<ENPago>();

        // Navigation: Documentos asociados
        public ICollection<ENDocumento> Documentos { get; set; } = new List<ENDocumento>();

        // ─── Business Logic Methods ───
        public bool IsActive() => Status == "activo" && EndDate >= DateTime.Now;
        public int RemainingMonths() => IsActive() ? (int)((EndDate - DateTime.Now).TotalDays / 30) : 0;
        public decimal TotalContractValue() => MonthlyRent * (decimal)((EndDate - StartDate).TotalDays / 30);
    }
}
