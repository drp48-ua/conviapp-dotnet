using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENFavorito — Entidad de Negocio para habitaciones o pisos guardados como favoritos.
    /// Permite al usuario acceder rápidamente a los alojamientos de su interés (Marina).
    /// </summary>
    public class ENFavorito
    {
        // ─── Atributos privados ───
        private int _id;
        private DateTime _fechaGuardado;
        private int _usuarioId;
        private int? _habitacionId;
        private int? _pisoId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        public DateTime FechaGuardado { get => _fechaGuardado; set => _fechaGuardado = value; }

        // ─── Claves foráneas ───
        public int UsuarioId { get => _usuarioId; set => _usuarioId = value; }
        public int? HabitacionId { get => _habitacionId; set => _habitacionId = value; }
        public int? PisoId { get => _pisoId; set => _pisoId = value; }

        // ─── Métodos de negocio ───
        public bool EsFavoritoHabitacion() => _habitacionId.HasValue;
        public bool EsFavoritoPiso() => _pisoId.HasValue;

        public ENFavorito() { _fechaGuardado = DateTime.Now; }
    }
}
