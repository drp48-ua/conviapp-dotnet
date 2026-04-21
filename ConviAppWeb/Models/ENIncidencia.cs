using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENIncidencia — Entidad de Negocio para incidencias o averías del piso.
    /// Gestión de problemas y seguimiento de su resolución (Nazim).
    /// </summary>
    public class ENIncidencia
    {
        // ─── Atributos privados ───
        private int _id;
        private string _titulo;
        private string _descripcion;
        private string _estado;
        private string _prioridad;
        private DateTime _fechaReporte;
        private DateTime? _fechaResolucion;
        private int _reportadaPorId;
        private int? _pisoId;
        private int? _habitacionId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get => _titulo; set => _titulo = value; }

        [Required]
        [MaxLength(2000)]
        public string Descripcion { get => _descripcion; set => _descripcion = value; }

        [Required]
        public string Estado { get => _estado; set => _estado = value; } // abierta, en_progreso, resuelta

        [MaxLength(20)]
        public string Prioridad { get => _prioridad; set => _prioridad = value; } // baja, media, alta, urgente

        public DateTime FechaReporte { get => _fechaReporte; set => _fechaReporte = value; }
        public DateTime? FechaResolucion { get => _fechaResolucion; set => _fechaResolucion = value; }

        // ─── Claves foráneas ───
        public int ReportadaPorId { get => _reportadaPorId; set => _reportadaPorId = value; }
        public ENUsuario? ReportadaPor { get; set; }

        public int? PisoId { get => _pisoId; set => _pisoId = value; }
        public ENPiso? Piso { get; set; }

        public int? HabitacionId { get => _habitacionId; set => _habitacionId = value; }
        public ENHabitacion? Habitacion { get; set; }

        // ─── Métodos de negocio ───
        public bool EstaResuelta() => _estado == "resuelta";
        public bool EsUrgente() => _prioridad == "urgente";
        public int? DiasAbierta() => _fechaResolucion.HasValue
            ? (int)(_fechaResolucion.Value - _fechaReporte).TotalDays
            : (int)(DateTime.Now - _fechaReporte).TotalDays;

        public ENIncidencia()
        {
            _fechaReporte = DateTime.Now;
            _estado = "abierta";
            _prioridad = "media";
        }
    }
}
