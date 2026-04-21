using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADGasto — Clase de Acceso a Datos para gastos compartidos (Nazim).</summary>
    public class CADGasto
    {
        private readonly ApplicationDbContext _context;
        public CADGasto(ApplicationDbContext context) => _context = context;

        public ENGasto Create(ENGasto g) { _context.Gastos.Add(g); _context.SaveChanges(); return g; }
        public ENGasto ReadById(int id) => _context.Gastos.Find(id);
        public List<ENGasto> ReadAll() => _context.Gastos.OrderByDescending(g => g.Fecha).ToList();
        public List<ENGasto> ReadByPiso(int pisoId) => _context.Gastos.Where(g => g.PisoId == pisoId).OrderByDescending(g => g.Fecha).ToList();
        public List<ENGasto> ReadByCategoria(int categoriaId) => _context.Gastos.Where(g => g.CategoriaId == categoriaId).ToList();
        public List<ENGasto> ReadByUsuario(int usuarioId) => _context.Gastos.Where(g => g.RegistradoPorId == usuarioId).ToList();

        public ENGasto Update(ENGasto g)
        {
            var ex = _context.Gastos.Find(g.Id);
            if (ex == null) return null;
            ex.Concepto = g.Concepto; ex.Importe = g.Importe; ex.Fecha = g.Fecha;
            ex.Pagado = g.Pagado; ex.Descripcion = g.Descripcion; ex.CategoriaId = g.CategoriaId;
            _context.SaveChanges();
            return ex;
        }

        public bool Delete(int id) { var g = _context.Gastos.Find(id); if (g == null) return false; _context.Gastos.Remove(g); _context.SaveChanges(); return true; }
        public decimal TotalByPiso(int pisoId) => _context.Gastos.Where(g => g.PisoId == pisoId).Sum(g => g.Importe);
    }
}
