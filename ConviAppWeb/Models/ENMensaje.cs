using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENMensaje — Entidad de Negocio para mensajes entre usuarios del piso.
    /// Facilita la comunicación interna (Maca).
    /// </summary>
    public class ENMensaje
    {
        // ─── Atributos privados ───
        private int _id;
        private string _contenido;
        private DateTime _fechaEnvio;
        private bool _leido;
        private int _emisorId;
        private int? _receptorId;
        private int? _pisoId; // mensajes de grupo del piso

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        [MaxLength(2000)]
        public string Contenido { get => _contenido; set => _contenido = value; }

        public DateTime FechaEnvio { get => _fechaEnvio; set => _fechaEnvio = value; }

        public bool Leido { get => _leido; set => _leido = value; }

        // ─── Claves foráneas ───
        public int EmisorId { get => _emisorId; set => _emisorId = value; }
        public ENUsuario? Emisor { get; set; }

        public int? ReceptorId { get => _receptorId; set => _receptorId = value; }
        public ENUsuario? Receptor { get; set; }

        public int? PisoId { get => _pisoId; set => _pisoId = value; }
        public ENPiso? Piso { get; set; }

        // ─── Métodos de negocio ───
        public bool EsMensajeGrupal() => _pisoId.HasValue && !_receptorId.HasValue;
        public bool EsMensajePrivado() => _receptorId.HasValue;
        public void MarcarComoLeido() => _leido = true;

        public ENMensaje()
        {
            _fechaEnvio = DateTime.Now;
            _leido = false;
        }
    }
}
