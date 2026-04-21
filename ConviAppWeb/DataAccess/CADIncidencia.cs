using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADIncidencia — Clase de Acceso a Datos para incidencias (Nazim).</summary>
    public class CADIncidencia
    {
        private readonly ApplicationDbContext _context;
        public CADIncidencia(ApplicationDbContext context) => _context = context;

        public ENIncidencia Create(ENIncidencia i) { _context.Incidencias.Add(i); _context.SaveChanges(); return i; }
        public ENIncidencia ReadById(int id) => _context.Incidencias.Find(id);
        public List<ENIncidencia> ReadAll() => _context.Incidencias.OrderByDescending(i => i.FechaReporte).ToList();
        public List<ENIncidencia> ReadByPiso(int pisoId) => _context.Incidencias.Where(i => i.PisoId == pisoId).ToList();
        public List<ENIncidencia> ReadAbiertas() => _context.Incidencias.Where(i => i.Estado != "resuelta").ToList();
        public List<ENIncidencia> ReadByUsuario(int usuarioId) => _context.Incidencias.Where(i => i.ReportadaPorId == usuarioId).ToList();

        public ENIncidencia Update(ENIncidencia i)
        {
            var ex = _context.Incidencias.Find(i.Id);
            if (ex == null) return null;
            ex.Titulo = i.Titulo; ex.Descripcion = i.Descripcion; ex.Estado = i.Estado;
            ex.Prioridad = i.Prioridad; ex.FechaResolucion = i.FechaResolucion;
            _context.SaveChanges();
            return ex;
        }

        public bool Delete(int id) { var i = _context.Incidencias.Find(id); if (i == null) return false; _context.Incidencias.Remove(i); _context.SaveChanges(); return true; }
        public int CountAbiertas() => _context.Incidencias.Count(i => i.Estado != "resuelta");
    }
}
