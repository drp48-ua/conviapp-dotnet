using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADPiso — Clase de Acceso a Datos para pisos (Marina).</summary>
    public class CADPiso
    {
        private readonly ApplicationDbContext _context;
        public CADPiso(ApplicationDbContext context) => _context = context;

        public ENPiso Create(ENPiso piso) { _context.Pisos.Add(piso); _context.SaveChanges(); return piso; }

        public ENPiso ReadById(int id) =>
            _context.Pisos.Include(p => p.Habitaciones).ThenInclude(h => h.Imagenes).FirstOrDefault(p => p.Id == id);

        public List<ENPiso> ReadAll() => _context.Pisos.Include(p => p.Habitaciones).ToList();

        public List<ENPiso> ReadDisponibles() =>
            _context.Pisos.Where(p => p.Disponible).Include(p => p.Habitaciones).ToList();

        public List<ENPiso> ReadByCiudad(string ciudad) =>
            _context.Pisos.Where(p => p.Ciudad.ToLower().Contains(ciudad.ToLower())).ToList();

        public ENPiso Update(ENPiso piso)
        {
            var ex = _context.Pisos.Find(piso.Id);
            if (ex == null) return null;
            ex.Direccion = piso.Direccion; ex.Ciudad = piso.Ciudad;
            ex.CodigoPostal = piso.CodigoPostal; ex.NumeroHabitaciones = piso.NumeroHabitaciones;
            ex.NumeroBanos = piso.NumeroBanos; ex.PrecioTotal = piso.PrecioTotal;
            ex.Descripcion = piso.Descripcion; ex.Disponible = piso.Disponible;
            _context.SaveChanges();
            return ex;
        }

        public bool Delete(int id) { var p = _context.Pisos.Find(id); if (p == null) return false; _context.Pisos.Remove(p); _context.SaveChanges(); return true; }
    }
}
