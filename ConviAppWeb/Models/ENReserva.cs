using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENReserva — Entidad de Negocio para reservas de zonas comunes.
    /// Permite organizar el uso de espacios compartidos (Lidia).
    /// </summary>
    public class ENReserva
    {
        // ─── Atributos privados ───
        private int _id;
        private DateTime _fechaInicio;
        private DateTime _fechaFin;
        private string _estado;
        private string _motivo;
        private int _usuarioId;
        private int _zonaComunId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        public DateTime FechaInicio { get => _fechaInicio; set => _fechaInicio = value; }

        [Required]
        public DateTime FechaFin { get => _fechaFin; set => _fechaFin = value; }

        [Required]
        public string Estado { get => _estado; set => _estado = value; } // pendiente, confirmada, cancelada

        [MaxLength(300)]
        public string? Motivo { get => _motivo; set => _motivo = value; }

        // ─── Claves foráneas ───
        public int UsuarioId { get => _usuarioId; set => _usuarioId = value; }
        public ENUsuario? Usuario { get; set; }

        public int ZonaComunId { get => _zonaComunId; set => _zonaComunId = value; }
        public ENZonaComun? ZonaComun { get; set; }

        // ─── Métodos de negocio ───
        public bool EsActiva() => _estado == "confirmada" && _fechaFin >= DateTime.Now;
        public bool EstaVencida() => _fechaFin < DateTime.Now;
        public double DuracionHoras() => (_fechaFin - _fechaInicio).TotalHours;

        public ENReserva() { _estado = "pendiente"; }
    }
}
