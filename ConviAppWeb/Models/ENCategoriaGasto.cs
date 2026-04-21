using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENCategoriaGasto — Categorías para clasificar los gastos comunes (Nazim).
    /// Ejemplos: Luz, Agua, Internet, Comunidad.
    /// </summary>
    public class ENCategoriaGasto
    {
        // ─── Atributos privados ───
        private int _id;
        private string _nombre;
        private string _descripcion;
        private string _icono; // nombre de icono (FontAwesome, etc.)

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get => _nombre; set => _nombre = value; }

        [MaxLength(300)]
        public string? Descripcion { get => _descripcion; set => _descripcion = value; }

        [MaxLength(50)]
        public string? Icono { get => _icono; set => _icono = value; }

        // ─── Navegación ───
        public ICollection<ENGasto> Gastos { get; set; } = new List<ENGasto>();

        // ─── Métodos de negocio ───
        public int TotalGastos() => Gastos?.Count ?? 0;
    }
}
