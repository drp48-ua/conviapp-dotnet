using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADTarea — Clase de Acceso a Datos para tareas domésticas (Maca).</summary>
    public class CADTarea
    {
        private readonly ApplicationDbContext _context;
        public CADTarea(ApplicationDbContext context) => _context = context;

        public ENTarea Create(ENTarea t) { _context.Tareas.Add(t); _context.SaveChanges(); return t; }
        public ENTarea ReadById(int id) => _context.Tareas.Include(t => t.AsignadaA).Include(t => t.CreadaPor).FirstOrDefault(t => t.Id == id);
        public List<ENTarea> ReadAll() => _context.Tareas.Include(t => t.AsignadaA).ToList();
        public List<ENTarea> ReadByUsuario(int usuarioId) => _context.Tareas.Where(t => t.AsignadaAId == usuarioId).ToList();
        public List<ENTarea> ReadByPiso(int pisoId) => _context.Tareas.Where(t => t.PisoId == pisoId).Include(t => t.AsignadaA).ToList();
        public List<ENTarea> ReadPendientes() => _context.Tareas.Where(t => t.Estado == "pendiente").ToList();

        public ENTarea Update(ENTarea t)
        {
            var ex = _context.Tareas.Find(t.Id);
            if (ex == null) return null;
            ex.Titulo = t.Titulo; ex.Descripcion = t.Descripcion; ex.Estado = t.Estado;
            ex.FechaLimite = t.FechaLimite; ex.Prioridad = t.Prioridad; ex.AsignadaAId = t.AsignadaAId;
            _context.SaveChanges();
            return ex;
        }

        public bool Delete(int id) { var t = _context.Tareas.Find(id); if (t == null) return false; _context.Tareas.Remove(t); _context.SaveChanges(); return true; }
    }
}
