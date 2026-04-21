using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADHabitacion — Clase de Acceso a Datos para habitaciones (Marina).</summary>
    public class CADHabitacion
    {
        private readonly ApplicationDbContext _context;
        public CADHabitacion(ApplicationDbContext context) => _context = context;

        public ENHabitacion Create(ENHabitacion h) { _context.Habitaciones.Add(h); _context.SaveChanges(); return h; }

        public ENHabitacion ReadById(int id) =>
            _context.Habitaciones.Include(h => h.Piso).Include(h => h.Imagenes).FirstOrDefault(h => h.Id == id);

        public List<ENHabitacion> ReadAll() =>
            _context.Habitaciones.Include(h => h.Piso).Include(h => h.Imagenes).ToList();

        public List<ENHabitacion> ReadByPiso(int pisoId) =>
            _context.Habitaciones.Where(h => h.PisoId == pisoId).Include(h => h.Imagenes).ToList();

        public List<ENHabitacion> ReadDisponibles() =>
            _context.Habitaciones.Where(h => h.Disponible).Include(h => h.Piso).Include(h => h.Imagenes).ToList();

        public ENHabitacion Update(ENHabitacion h)
        {
            var ex = _context.Habitaciones.Find(h.Id);
            if (ex == null) return null;
            ex.Numero = h.Numero; ex.Precio = h.Precio; ex.Metros = h.Metros;
            ex.Disponible = h.Disponible; ex.TieneBano = h.TieneBano; ex.Descripcion = h.Descripcion;
            _context.SaveChanges();
            return ex;
        }

        public bool Delete(int id) { var h = _context.Habitaciones.Find(id); if (h == null) return false; _context.Habitaciones.Remove(h); _context.SaveChanges(); return true; }
        public int CountDisponibles() => _context.Habitaciones.Count(h => h.Disponible);
    }
}
