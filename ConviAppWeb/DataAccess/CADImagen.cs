using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADImagen — Clase de Acceso a Datos para imágenes (Marina).</summary>
    public class CADImagen
    {
        private readonly ApplicationDbContext _context;
        public CADImagen(ApplicationDbContext context) => _context = context;

        public ENImagen Create(ENImagen img) { _context.Imagenes.Add(img); _context.SaveChanges(); return img; }
        public ENImagen ReadById(int id) => _context.Imagenes.Find(id);
        public List<ENImagen> ReadByHabitacion(int habitacionId) => _context.Imagenes.Where(i => i.HabitacionId == habitacionId).ToList();
        public List<ENImagen> ReadByPiso(int pisoId) => _context.Imagenes.Where(i => i.PisoId == pisoId).ToList();
        public ENImagen ReadPrincipalByHabitacion(int habitacionId) => _context.Imagenes.FirstOrDefault(i => i.HabitacionId == habitacionId && i.EsPrincipal);

        public bool Delete(int id) { var i = _context.Imagenes.Find(id); if (i == null) return false; _context.Imagenes.Remove(i); _context.SaveChanges(); return true; }
    }
}
