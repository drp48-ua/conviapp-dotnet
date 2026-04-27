using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENRol — Entidad de Negocio para roles de usuario.
    /// Permite controlar permisos: inquilino, administrador, propietario (Moni).
    /// </summary>
    public class ENRol
    {
        // ─── Atributos privados ───
        private int _id;
        private string _nombre;
        private string _descripcion;
        private bool _puedeGestionarPisos;
        private bool _puedeVerContratos;
        private bool _puedeGestionarUsuarios;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get => _nombre; set => _nombre = value; } // inquilino, administrador, propietario

        [MaxLength(250)]
        public string? Descripcion { get => _descripcion; set => _descripcion = value; }

        public bool PuedeGestionarPisos { get => _puedeGestionarPisos; set => _puedeGestionarPisos = value; }
        public bool PuedeVerContratos { get => _puedeVerContratos; set => _puedeVerContratos = value; }
        public bool PuedeGestionarUsuarios { get => _puedeGestionarUsuarios; set => _puedeGestionarUsuarios = value; }


        // ─── Métodos de negocio ───
        public bool EsAdministrador() => _nombre.ToLower() == "administrador";
        public bool EsInquilino() => _nombre.ToLower() == "inquilino";
    }
}
