using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENTarea — Entidad de Negocio para tareas domésticas compartidas.
    /// Organiza responsabilidades entre inquilinos (Maca).
    /// </summary>
    public class ENTarea
    {
        // ─── Atributos privados ───
        private int _id;
        private string _titulo;
        private string _descripcion;
        private string _estado;
        private DateTime _fechaCreacion;
        private DateTime? _fechaLimite;
        private string _prioridad;
        private int _creadaPorId;
        private int? _asignadaAId;
        private int? _pisoId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get => _titulo; set => _titulo = value; }

        [MaxLength(1000)]
        public string? Descripcion { get => _descripcion; set => _descripcion = value; }

        [Required]
        public string Estado { get => _estado; set => _estado = value; } // pendiente, en_progreso, completada

        public DateTime FechaCreacion { get => _fechaCreacion; set => _fechaCreacion = value; }

        public DateTime? FechaLimite { get => _fechaLimite; set => _fechaLimite = value; }

        [MaxLength(20)]
        public string Prioridad { get => _prioridad; set => _prioridad = value; } // baja, media, alta

        // ─── Claves foráneas ───
        public int CreadaPorId { get => _creadaPorId; set => _creadaPorId = value; }
        public ENUsuario? CreadaPor { get; set; }

        public int? AsignadaAId { get => _asignadaAId; set => _asignadaAId = value; }
        public ENUsuario? AsignadaA { get; set; }

        public int? PisoId { get => _pisoId; set => _pisoId = value; }
        public ENPiso? Piso { get; set; }

        // ─── Métodos de negocio ───
        public bool EstaCompletada() => _estado == "completada";
        public bool EstaVencida() => _fechaLimite.HasValue && _fechaLimite.Value < DateTime.Now && !EstaCompletada();

        public ENTarea()
        {
            _fechaCreacion = DateTime.Now;
            _estado = "pendiente";
            _prioridad = "media";
        }
    }
}
