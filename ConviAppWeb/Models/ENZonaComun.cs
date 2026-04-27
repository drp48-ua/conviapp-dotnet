using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENZonaComun — Entidad de Negocio para zonas comunes del piso.
    /// Ejemplos: lavandería, cocina, sala de estudio (Lidia).
    /// </summary>
    public class ENZonaComun
    {
        // ─── Atributos privados ───
        private int _id;
        private string _nombre;
        private string _descripcion;
        private int _capacidad;
        private bool _disponible;
        private int? _pisoId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get => _nombre; set => _nombre = value; } // lavandería, cocina, sala_estudio

        [MaxLength(500)]
        public string? Descripcion { get => _descripcion; set => _descripcion = value; }

        [Range(1, 100)]
        public int Capacidad { get => _capacidad; set => _capacidad = value; }

        public bool Disponible { get => _disponible; set => _disponible = value; }

        // ─── Clave foránea ───
        public int? PisoId { get => _pisoId; set => _pisoId = value; }

        // ─── Métodos de negocio ───
        public bool EstaDisponible() => _disponible;
        public int TotalReservas() => 0; // Se calcularía con una consulta ADO.NET

        public ENZonaComun() { _disponible = true; _capacidad = 1; }
    }
}
