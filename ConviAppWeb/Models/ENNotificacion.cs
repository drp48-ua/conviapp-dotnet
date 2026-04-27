using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENNotificacion — Entidad de Negocio para notificaciones automáticas.
    /// Envía avisos sobre pagos pendientes, tareas sin realizar, incidencias (Lidia).
    /// </summary>
    public class ENNotificacion
    {
        // ─── Atributos privados ───
        private int _id;
        private string _titulo;
        private string _mensaje;
        private string _tipo;
        private bool _leida;
        private DateTime _fechaCreacion;
        private DateTime? _fechaLectura;
        private int _usuarioId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get => _titulo; set => _titulo = value; }

        [Required]
        [MaxLength(1000)]
        public string Mensaje { get => _mensaje; set => _mensaje = value; }

        [Required]
        [MaxLength(50)]
        public string Tipo { get => _tipo; set => _tipo = value; } // pago, tarea, incidencia, reserva, general

        public bool Leida { get => _leida; set => _leida = value; }

        public DateTime FechaCreacion { get => _fechaCreacion; set => _fechaCreacion = value; }

        public DateTime? FechaLectura { get => _fechaLectura; set => _fechaLectura = value; }

        // ─── Clave foránea ───
        public int UsuarioId { get => _usuarioId; set => _usuarioId = value; }

        // ─── Métodos de negocio ───
        public bool EsLeida() => _leida;
        public void MarcarComoLeida() { _leida = true; _fechaLectura = DateTime.Now; }
        public bool EsReciente() => (DateTime.Now - _fechaCreacion).TotalHours < 24;

        public ENNotificacion()
        {
            _fechaCreacion = DateTime.Now;
            _leida = false;
        }
    }
}
