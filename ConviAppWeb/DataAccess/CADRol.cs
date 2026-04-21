using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADRol — Clase de Acceso a Datos para roles (Moni).</summary>
    public class CADRol
    {
        private readonly ApplicationDbContext _context;
        public CADRol(ApplicationDbContext context) => _context = context;

        public ENRol Create(ENRol rol) { _context.Roles.Add(rol); _context.SaveChanges(); return rol; }
        public ENRol ReadById(int id) => _context.Roles.Find(id);
        public List<ENRol> ReadAll() => _context.Roles.OrderBy(r => r.Nombre).ToList();
        public ENRol ReadByNombre(string nombre) => _context.Roles.FirstOrDefault(r => r.Nombre == nombre);

        public ENRol Update(ENRol rol)
        {
            var ex = _context.Roles.Find(rol.Id);
            if (ex == null) return null;
            ex.Nombre = rol.Nombre;
            ex.Descripcion = rol.Descripcion;
            ex.PuedeGestionarPisos = rol.PuedeGestionarPisos;
            ex.PuedeVerContratos = rol.PuedeVerContratos;
            ex.PuedeGestionarUsuarios = rol.PuedeGestionarUsuarios;
            _context.SaveChanges();
            return ex;
        }

        public bool Delete(int id)
        {
            var rol = _context.Roles.Find(id);
            if (rol == null) return false;
            _context.Roles.Remove(rol);
            _context.SaveChanges();
            return true;
        }
    }
}
