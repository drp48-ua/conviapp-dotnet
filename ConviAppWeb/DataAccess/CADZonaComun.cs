using System;
using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADZonaComun — Clase de Acceso a Datos para zonas comunes (Lidia).</summary>
    public class CADZonaComun
    {
        private readonly ApplicationDbContext _context;
        public CADZonaComun(ApplicationDbContext context) => _context = context;

        public ENZonaComun Create(ENZonaComun z) { _context.ZonasComunes.Add(z); _context.SaveChanges(); return z; }
        public ENZonaComun ReadById(int id) => _context.ZonasComunes.Find(id);
        public List<ENZonaComun> ReadAll() => _context.ZonasComunes.ToList();
        public List<ENZonaComun> ReadByPiso(int pisoId) => _context.ZonasComunes.Where(z => z.PisoId == pisoId).ToList();
        public List<ENZonaComun> ReadDisponibles() => _context.ZonasComunes.Where(z => z.Disponible).ToList();

        public ENZonaComun Update(ENZonaComun z)
        {
            var ex = _context.ZonasComunes.Find(z.Id);
            if (ex == null) return null;
            ex.Nombre = z.Nombre; ex.Descripcion = z.Descripcion;
            ex.Capacidad = z.Capacidad; ex.Disponible = z.Disponible;
            _context.SaveChanges();
            return ex;
        }

        public bool Delete(int id) { var z = _context.ZonasComunes.Find(id); if (z == null) return false; _context.ZonasComunes.Remove(z); _context.SaveChanges(); return true; }
    }
}
