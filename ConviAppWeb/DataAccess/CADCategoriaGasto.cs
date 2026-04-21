using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADCategoriaGasto — Clase de Acceso a Datos para categorías de gasto (Nazim).</summary>
    public class CADCategoriaGasto
    {
        private readonly ApplicationDbContext _context;
        public CADCategoriaGasto(ApplicationDbContext context) => _context = context;

        public ENCategoriaGasto Create(ENCategoriaGasto c) { _context.CategoriasGasto.Add(c); _context.SaveChanges(); return c; }
        public ENCategoriaGasto ReadById(int id) => _context.CategoriasGasto.Find(id);
        public List<ENCategoriaGasto> ReadAll() => _context.CategoriasGasto.OrderBy(c => c.Nombre).ToList();
        public ENCategoriaGasto Update(ENCategoriaGasto c) { var ex = _context.CategoriasGasto.Find(c.Id); if (ex == null) return null; ex.Nombre = c.Nombre; ex.Descripcion = c.Descripcion; ex.Icono = c.Icono; _context.SaveChanges(); return ex; }
        public bool Delete(int id) { var c = _context.CategoriasGasto.Find(id); if (c == null) return false; _context.CategoriasGasto.Remove(c); _context.SaveChanges(); return true; }
    }
}
