using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENContrato — Entidad de Negocio para contratos de arrendamiento.
    /// Capa de lógica de negocio (Entrega 3 - Dani).
    /// Incluye atributos privados, propiedades públicas y estructura de comisiones.
    /// </summary>
    public class ENContrato
    {
        // ─── Atributos privados ───
        private int _id;
        private string _type;
        private DateTime _startDate;
        private DateTime _endDate;
        private decimal _monthlyRent;
        private decimal _depositAmount;
        private string _status;
        private string _notes;
        private int _propertyId;
        private int _userId;
        private decimal _commissionRate; // % comisión sobre renta mensual (ej. 5 = 5%)

        // ─── Propiedades públicas ───
        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [Required(ErrorMessage = "El tipo de contrato es obligatorio")]
        public string Type
        {
            get => _type;
            set => _type = value;
        } // arrendamiento, temporal, subarriendo

        [Required]
        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value;
        }

        [Required]
        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = value;
        }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "La renta debe ser positiva")]
        public decimal MonthlyRent
        {
            get => _monthlyRent;
            set => _monthlyRent = value;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositAmount
        {
            get => _depositAmount;
            set => _depositAmount = value;
        }

        [Required]
        public string Status
        {
            get => _status;
            set => _status = value;
        } // activo, vencido, cancelado

        public string? Notes
        {
            get => _notes;
            set => _notes = value;
        }

        /// <summary>Porcentaje de comisión sobre la renta mensual (ej: 5 = 5%).</summary>
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal CommissionRate
        {
            get => _commissionRate;
            set => _commissionRate = value;
        }

        // ─── Claves foráneas ───
        public int PropertyId
        {
            get => _propertyId;
            set => _propertyId = value;
        }
        // Property nav eliminada (sin EF)

        public int UserId
        {
            get => _userId;
            set => _userId = value;
        }
        // User nav eliminada (sin EF)

        // ─── Navegación ───
        public ICollection<ENPago> Pagos { get; set; } = new List<ENPago>();
        public ICollection<ENDocumento> Documentos { get; set; } = new List<ENDocumento>();

        // ─── Métodos de negocio ───
        public bool IsActive() => _status == "activo" && _endDate >= DateTime.Now;

        public int RemainingMonths() =>
            IsActive() ? (int)((_endDate - DateTime.Now).TotalDays / 30) : 0;

        public decimal TotalContractValue() =>
            _monthlyRent * (decimal)((_endDate - _startDate).TotalDays / 30);

        /// <summary>Calcula la comisión mensual según el porcentaje configurado.</summary>
        public decimal CalculateMonthlyCommission() =>
            _monthlyRent * (_commissionRate / 100m);

        /// <summary>Calcula la comisión total durante toda la vigencia del contrato.</summary>
        public decimal CalculateTotalCommission() =>
            TotalContractValue() * (_commissionRate / 100m);

        /// <summary>Constructor por defecto — valores iniciales.</summary>
        public ENContrato()
        {
            _status = "activo";
            _commissionRate = 0m;
        }
    }
}
