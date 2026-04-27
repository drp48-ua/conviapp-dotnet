using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENHabitacion — Entidad de Negocio para habitaciones dentro de un piso.
    /// Es la unidad principal de alquiler (Marina).
    /// </summary>
    public class ENHabitacion
    {
        // ─── Atributos privados ───
        private int _id;
        private string _numero;
        private decimal _precio;
        private double _metros;
        private bool _disponible;
        private bool _tieneBano;
        private string _descripcion;
        private int _pisoId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        [MaxLength(20)]
        public string Numero { get => _numero; set => _numero = value; } // "101", "A", etc.

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Precio { get => _precio; set => _precio = value; }

        [Range(0, 500)]
        public double Metros { get => _metros; set => _metros = value; }

        public bool Disponible { get => _disponible; set => _disponible = value; }

        public bool TieneBano { get => _tieneBano; set => _tieneBano = value; }

        [MaxLength(1000)]
        public string? Descripcion { get => _descripcion; set => _descripcion = value; }

        // ─── Clave foránea ───
        public int PisoId { get => _pisoId; set => _pisoId = value; }

        // ─── Métodos de negocio ───
        public bool EstaLibre() => _disponible;
        public string DescripcionCorta() => $"Hab. {_numero} — {_precio:C}/mes — {_metros}m²";
    }
}
