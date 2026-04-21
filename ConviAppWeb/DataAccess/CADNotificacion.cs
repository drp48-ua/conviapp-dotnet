using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADNotificacion — Clase de Acceso a Datos para notificaciones (Lidia).</summary>
    public class CADNotificacion
    {
        private readonly ApplicationDbContext _context;
        public CADNotificacion(ApplicationDbContext context) => _context = context;

        public ENNotificacion Create(ENNotificacion n) { _context.Notificaciones.Add(n); _context.SaveChanges(); return n; }
        public ENNotificacion ReadById(int id) => _context.Notificaciones.Find(id);
        public List<ENNotificacion> ReadAll() => _context.Notificaciones.OrderByDescending(n => n.FechaCreacion).ToList();
        public List<ENNotificacion> ReadByUsuario(int usuarioId) => _context.Notificaciones.Where(n => n.UsuarioId == usuarioId).OrderByDescending(n => n.FechaCreacion).ToList();
        public List<ENNotificacion> ReadNoLeidas(int usuarioId) => _context.Notificaciones.Where(n => n.UsuarioId == usuarioId && !n.Leida).ToList();

        public bool MarcarLeida(int id)
        {
            var n = _context.Notificaciones.Find(id);
            if (n == null) return false;
            n.MarcarComoLeida();
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id) { var n = _context.Notificaciones.Find(id); if (n == null) return false; _context.Notificaciones.Remove(n); _context.SaveChanges(); return true; }
        public int CountNoLeidas(int usuarioId) => _context.Notificaciones.Count(n => n.UsuarioId == usuarioId && !n.Leida);
    }
}
