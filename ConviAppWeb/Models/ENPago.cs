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
        // ─── Atributos privados ───
        private int _id;
        private decimal _amount;
        private DateTime _date;
        private string _method;
        private string _status;
        private string _concept;
        private string _reference;
        private int _contratoId;
        private int _userId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El importe debe ser mayor que 0")]
        public decimal Amount
        {
            get => _amount;
            set => _amount = value;
        }

        [Required]
        public DateTime Date
        {
            get => _date;
            set => _date = value;
        }

        [Required(ErrorMessage = "El método de pago es obligatorio")]
        public string Method
        {
            get => _method;
            set => _method = value;
        } // transferencia, efectivo, tarjeta, bizum

        [Required]
        public string Status
        {
            get => _status;
            set => _status = value;
        } // pagado, pendiente, rechazado

        public string? Concept
        {
            get => _concept;
            set => _concept = value;
        } // Alquiler Abril, Depósito, etc.

        public string? Reference
        {
            get => _reference;
            set => _reference = value;
        } // Referencia bancaria

        // ─── Claves foráneas ───
        public int ContratoId
        {
            get => _contratoId;
            set => _contratoId = value;
        }
        public ENContrato? Contrato { get; set; }

        public int UserId
        {
            get => _userId;
            set => _userId = value;
        }
        // User nav eliminada (sin EF)

        // ─── Métodos de negocio ───
        public bool IsPaid() => _status == "pagado";
        public bool IsOverdue() => _status == "pendiente" && _date < DateTime.Now;
        public bool IsRejected() => _status == "rechazado";

        /// <summary>Constructor por defecto.</summary>
        public ENPago()
        {
            _status = "pendiente";
            _date = DateTime.Now;
        }
    }
}
