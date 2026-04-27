using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENImagen — Entidad de Negocio para imágenes de habitaciones y pisos.
    /// Permite mostrar fotos del alojamiento (Marina).
    /// </summary>
    public class ENImagen
    {
        // ─── Atributos privados ───
        private int _id;
        private string _url;
        private string _descripcion;
        private bool _esPrincipal;
        private DateTime _fechaSubida;
        private int? _habitacionId;
        private int? _pisoId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        [MaxLength(500)]
        public string Url { get => _url; set => _url = value; }

        [MaxLength(200)]
        public string? Descripcion { get => _descripcion; set => _descripcion = value; }

        public bool EsPrincipal { get => _esPrincipal; set => _esPrincipal = value; }

        public DateTime FechaSubida { get => _fechaSubida; set => _fechaSubida = value; }

        // ─── Claves foráneas ───
        public int? HabitacionId { get => _habitacionId; set => _habitacionId = value; }
        public int? PisoId { get => _pisoId; set => _pisoId = value; }

        // ─── Métodos de negocio ───
        public bool EsImagenPrincipal() => _esPrincipal;

        public ENImagen() { _fechaSubida = DateTime.Now; }
    }
}
