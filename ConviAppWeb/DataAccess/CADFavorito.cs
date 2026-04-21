using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADFavorito — Clase de Acceso a Datos para favoritos (Marina).</summary>
    public class CADFavorito
    {
        private readonly ApplicationDbContext _context;
        public CADFavorito(ApplicationDbContext context) => _context = context;

        public ENFavorito Create(ENFavorito f) { _context.Favoritos.Add(f); _context.SaveChanges(); return f; }
        public List<ENFavorito> ReadByUsuario(int usuarioId) => _context.Favoritos.Where(f => f.UsuarioId == usuarioId).ToList();
        public bool EsFavorito(int usuarioId, int? habitacionId, int? pisoId) =>
            _context.Favoritos.Any(f => f.UsuarioId == usuarioId && f.HabitacionId == habitacionId && f.PisoId == pisoId);
        public bool Delete(int id) { var f = _context.Favoritos.Find(id); if (f == null) return false; _context.Favoritos.Remove(f); _context.SaveChanges(); return true; }
    }
}
