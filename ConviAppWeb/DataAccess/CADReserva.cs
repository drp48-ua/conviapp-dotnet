using System;
using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADReserva — Clase de Acceso a Datos para reservas de zonas comunes (Lidia).</summary>
    public class CADReserva
    {
        private readonly ApplicationDbContext _context;
        public CADReserva(ApplicationDbContext context) => _context = context;

        public ENReserva Create(ENReserva r) { _context.Reservas.Add(r); _context.SaveChanges(); return r; }
        public ENReserva ReadById(int id) => _context.Reservas.Find(id);
        public List<ENReserva> ReadAll() => _context.Reservas.OrderByDescending(r => r.FechaInicio).ToList();
        public List<ENReserva> ReadByUsuario(int usuarioId) => _context.Reservas.Where(r => r.UsuarioId == usuarioId).ToList();
        public List<ENReserva> ReadByZonaComun(int zonaComunId) => _context.Reservas.Where(r => r.ZonaComunId == zonaComunId).ToList();
        public List<ENReserva> ReadActivas() => _context.Reservas.Where(r => r.Estado == "confirmada" && r.FechaFin >= DateTime.Now).ToList();

        public ENReserva Update(ENReserva r)
        {
            var ex = _context.Reservas.Find(r.Id);
            if (ex == null) return null;
            ex.FechaInicio = r.FechaInicio; ex.FechaFin = r.FechaFin;
            ex.Estado = r.Estado; ex.Motivo = r.Motivo;
            _context.SaveChanges();
            return ex;
        }

        public bool Delete(int id) { var r = _context.Reservas.Find(id); if (r == null) return false; _context.Reservas.Remove(r); _context.SaveChanges(); return true; }
    }
}
