using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// CADUsuario — Clase de Acceso a Datos para usuarios.
    /// CRUD + consultas de negocio (Moni).
    /// </summary>
    public class CADUsuario
    {
        private readonly ApplicationDbContext _context;
        public CADUsuario(ApplicationDbContext context) => _context = context;

        public ENUsuario Create(ENUsuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return usuario;
        }

        public ENUsuario ReadById(int id) =>
            _context.Usuarios.Include(u => u.Rol).Include(u => u.Suscripcion)
                .FirstOrDefault(u => u.Id == id);

        public List<ENUsuario> ReadAll() =>
            _context.Usuarios.Include(u => u.Rol).OrderBy(u => u.Nombre).ToList();

        public ENUsuario ReadByEmail(string email) =>
            _context.Usuarios.Include(u => u.Rol)
                .FirstOrDefault(u => u.Email == email);

        public List<ENUsuario> ReadByRol(int rolId) =>
            _context.Usuarios.Where(u => u.RolId == rolId).ToList();

        public ENUsuario Update(ENUsuario usuario)
        {
            var existing = _context.Usuarios.Find(usuario.Id);
            if (existing == null) return null;
            existing.Nombre = usuario.Nombre;
            existing.Apellidos = usuario.Apellidos;
            existing.Email = usuario.Email;
            existing.Telefono = usuario.Telefono;
            existing.Activo = usuario.Activo;
            existing.RolId = usuario.RolId;
            _context.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return false;
            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();
            return true;
        }

        public bool ExisteEmail(string email) =>
            _context.Usuarios.Any(u => u.Email == email);
    }
}
