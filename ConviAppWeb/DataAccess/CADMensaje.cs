using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADMensaje — Clase de Acceso a Datos para mensajes internos (Maca).</summary>
    public class CADMensaje
    {
        private readonly ApplicationDbContext _context;
        public CADMensaje(ApplicationDbContext context) => _context = context;

        public ENMensaje Create(ENMensaje m) { _context.Mensajes.Add(m); _context.SaveChanges(); return m; }
        public ENMensaje ReadById(int id) => _context.Mensajes.Find(id);
        public List<ENMensaje> ReadByEmisor(int emisorId) => _context.Mensajes.Where(m => m.EmisorId == emisorId).OrderByDescending(m => m.FechaEnvio).ToList();
        public List<ENMensaje> ReadByReceptor(int receptorId) => _context.Mensajes.Where(m => m.ReceptorId == receptorId).OrderByDescending(m => m.FechaEnvio).ToList();
        public List<ENMensaje> ReadByPiso(int pisoId) => _context.Mensajes.Where(m => m.PisoId == pisoId).OrderByDescending(m => m.FechaEnvio).ToList();
        public List<ENMensaje> ReadNoLeidos(int receptorId) => _context.Mensajes.Where(m => m.ReceptorId == receptorId && !m.Leido).ToList();
        public bool Delete(int id) { var m = _context.Mensajes.Find(id); if (m == null) return false; _context.Mensajes.Remove(m); _context.SaveChanges(); return true; }
        public int CountNoLeidos(int receptorId) => _context.Mensajes.Count(m => m.ReceptorId == receptorId && !m.Leido);
    }
}
