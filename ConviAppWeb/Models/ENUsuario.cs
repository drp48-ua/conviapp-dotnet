using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENUsuario — Entidad de Negocio para usuarios de la plataforma.
    /// Representa a inquilinos y administradores (Moni).
    /// </summary>
    public class ENUsuario
    {
        // ─── Atributos privados ───
        private int _id;
        private string _nombre;
        private string _apellidos;
        private string _email;
        private string _passwordHash;
        private string _telefono;
        private DateTime _fechaRegistro;
        private bool _activo;
        private int _rolId;
        private int? _suscripcionId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id { get => _id; set => _id = value; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get => _nombre; set => _nombre = value; }

        [MaxLength(150)]
        public string Apellidos { get => _apellidos; set => _apellidos = value; }

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get => _email; set => _email = value; }

        [Required]
        public string PasswordHash { get => _passwordHash; set => _passwordHash = value; }

        [Phone]
        [MaxLength(20)]
        public string? Telefono { get => _telefono; set => _telefono = value; }

        public DateTime FechaRegistro { get => _fechaRegistro; set => _fechaRegistro = value; }

        public bool Activo { get => _activo; set => _activo = value; }

        // ─── Claves foráneas ───
        public int RolId { get => _rolId; set => _rolId = value; }
        public ENRol? Rol { get; set; }

        public int? SuscripcionId { get => _suscripcionId; set => _suscripcionId = value; }
        public ENSuscripcion? Suscripcion { get; set; }

        // ─── Navegación ───
        public ICollection<ENContrato> Contratos { get; set; } = new List<ENContrato>();
        public ICollection<ENPago> Pagos { get; set; } = new List<ENPago>();

        // ─── Métodos de negocio ───
        public string NombreCompleto() => $"{_nombre} {_apellidos}".Trim();
        public bool TieneSuscripcionActiva() => _suscripcionId.HasValue && _activo;

        public ENUsuario()
        {
            _fechaRegistro = DateTime.Now;
            _activo = true;
        }
    }
}
