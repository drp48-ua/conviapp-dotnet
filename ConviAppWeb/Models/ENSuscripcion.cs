using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENSuscripcion — Entidad de Negocio para planes de suscripción.
    /// Modelo de monetización: Básico, Profesional, Enterprise (Moni).
    /// </summary>
    public class ENSuscripcion
    {
        // ─── Atributos privados ───
        private int _id;
        private string _plan;
        private decimal _precioMensual;
        private DateTime _fechaInicio;
        private DateTime _fechaFin;
        private bool _activa;
        private int _usuarioId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        [MaxLength(50)]
        public string Plan { get => _plan; set => _plan = value; } // basico, profesional, enterprise

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal PrecioMensual { get => _precioMensual; set => _precioMensual = value; }

        [Required]
        public DateTime FechaInicio { get => _fechaInicio; set => _fechaInicio = value; }

        [Required]
        public DateTime FechaFin { get => _fechaFin; set => _fechaFin = value; }

        public bool Activa { get => _activa; set => _activa = value; }

        // ─── Clave foránea ───
        public int UsuarioId { get => _usuarioId; set => _usuarioId = value; }

        // ─── Métodos de negocio ───
        public bool EsValida() => _activa && _fechaFin >= DateTime.Now;
        public int DiasRestantes() => EsValida() ? (int)(_fechaFin - DateTime.Now).TotalDays : 0;
        public bool EsPremium() => _plan.ToLower() == "profesional" || _plan.ToLower() == "enterprise";

        public ENSuscripcion()
        {
            _fechaInicio = DateTime.Now;
            _activa = true;
        }
    }
}
